using Consul;
using DnsClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resilience;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using zipkin4net;
using zipkin4net.Middleware;
using zipkin4net.Tracers;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;
using zsq.ContactApi.Config;
using zsq.ContactApi.Data;
using zsq.ContactApi.Infrastructure;
using zsq.ContactApi.IntergrationEvents.EventsHandler;
using zsq.ContactApi.Services;

namespace zsq.ContactApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoConnectionConfig>(Configuration.GetSection("MongoConnection"));

            services.AddScoped<IContactRepository, ContactRepository>()
                .AddScoped<IContactApplyRequestRepository, ContactApplyRequestRepository>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<ContactContext>()
                .AddScoped<UserProfileChangedEventHandler>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = "contact_api";
                    options.Authority = "http://localhost:6232";//使用网关地址，会自动转发
                    options.SaveToken = true;//会把token存在header里
                });

            //不加这个会报错
            //No service for type 'Microsoft.AspNetCore.Http.IHttpContextAccessor' has been registered.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(typeof(ResilienceHttpClientFactory), sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ResilienceHttpClient>>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["HttpClientRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["HttpClientRetryCount"]);
                }
                var exceptionCount = 5;
                if (!string.IsNullOrEmpty(Configuration["HttpClientExceptionCountAllowedBeforeBreaking"]))
                {
                    exceptionCount = int.Parse(Configuration["HttpClientExceptionCountAllowedBeforeBreaking"]);
                }
                return new ResilienceHttpClientFactory(logger, httpContextAccessor, retryCount, exceptionCount);
            });

            services.AddSingleton<IHttpClient>(sp =>
            {
                return sp.GetRequiredService<ResilienceHttpClientFactory>().GetResilienceHttpClient();
            });

            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;

                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));

            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                return new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCap(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("MysqlContact"))
                    .UseRabbitMQ("localhost")
                    .UseDashboard()
                    .UseDiscovery(c =>
                    {
                        c.DiscoveryServerHostName = "localhost";
                        c.DiscoveryServerPort = 8500;
                        c.CurrentNodeHostName = "localhost";
                        c.CurrentNodePort = 6233;
                        c.NodeId = "2";
                        c.NodeName = "contact cap 2 node";
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime lifetime, IOptions<ServiceDiscoveryOptions> serviceOptions,
            IConsulClient consul, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            lifetime.ApplicationStarted.Register(() =>
            {
                RegisterService(app, serviceOptions, consul);
            });
            lifetime.ApplicationStopped.Register(() =>
            {
                DeregisterService(app, serviceOptions, consul);
            });

            RegisterZipkinService(app, loggerFactory, lifetime);
            app.UseAuthentication();
            app.UseMvc();
        }

        private void RegisterService(IApplicationBuilder app,
            IOptions<ServiceDiscoveryOptions> serviceOptions,
            IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ContactServiceName}_{address.Host}:{address.Port}";

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "HealthCheck").OriginalString
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = address.Host,
                    ID = serviceId,
                    Name = serviceOptions.Value.ContactServiceName,
                    Port = address.Port
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();
            }
        }

        private void DeregisterService(IApplicationBuilder app, IOptions<ServiceDiscoveryOptions> serviceOptions,
            IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ContactServiceName}_{address.Host}:{address.Port}";
                consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
            }
        }

        public void RegisterZipkinService(IApplicationBuilder app,
            ILoggerFactory loggerFactory,
            IApplicationLifetime lifetime)
        {
            lifetime.ApplicationStarted.Register(() =>
            {
                //记录数据密度，1.0代表全部记录
                TraceManager.SamplingRate = 1.0f;
                //数据保存到内存
                var logger = new TracingLogger(loggerFactory, "zipkin4net");
                var httpSender = new HttpZipkinSender("http://localhost:9411", "application/json");
                var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer(), new Statistics());

                var consoleTracer = new ConsoleTracer();
                TraceManager.RegisterTracer(tracer);
                TraceManager.RegisterTracer(consoleTracer);
                TraceManager.Start(logger);
            });

            lifetime.ApplicationStopped.Register(() =>
            {
                TraceManager.Stop();
            });
            app.UseTracing("contact_api");
        }
    }
}
