using Consul;
using DnsClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resilience;
using System;
using System.IdentityModel.Tokens.Jwt;
using zipkin4net;
using zipkin4net.Middleware;
using zipkin4net.Tracers;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;
using zsq.RecommendApi.Config;
using zsq.RecommendApi.Data;
using zsq.RecommendApi.Infrastructure;
using zsq.RecommendApi.IntegrationEventHandlers;
using zsq.RecommendApi.Services;

namespace zsq.RecommendApi
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
            services.AddDbContext<RecommendContext>(o =>
            {
                o.UseMySQL(Configuration.GetConnectionString("MysqlRecommend"));
            });

            services.AddScoped<IUserService, UserService>()
                .AddScoped<IContactService, ContactService>()
                .AddScoped<ProjectCreatedIntegrationEventHandler>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = "recommend_api";
                    options.Authority = "http://localhost:6232";//使用网关地址，会自动转发
                    options.SaveToken = true;//会把token存在header里
                });

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
                options.UseMySql(Configuration.GetConnectionString("MysqlRecommend"))
                    .UseRabbitMQ("localhost")
                    .UseDashboard()
                    .UseDiscovery(c =>
                    {
                        c.DiscoveryServerHostName = "localhost";
                        c.DiscoveryServerPort = 8500;
                        c.CurrentNodeHostName = "localhost";
                        c.CurrentNodePort = 6235;
                        c.NodeId = "4";
                        c.NodeName = "recommend cap 4 node";
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            RegisterZipkinService(app, loggerFactory, lifetime);
            app.UseAuthentication();
            app.UseMvc();
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
            app.UseTracing("recommend_api");
        }
    }
}
