using DnsClient;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using zsq.Project.Domain.AggregatesModel;
using zsq.Project.Infrastructure;
using zsq.Project.Infrastructure.Repository;
using zsq.ProjectApi.Applications.Queries;
using zsq.ProjectApi.Config;
using zsq.ProjectApi.Services;

namespace zsq.ProjectApi
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
            services.AddDbContext<ProjectContext>(o =>
            {
                o.UseMySQL(Configuration.GetConnectionString("MysqlProject"), sql =>
                 {
                     sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                 });
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = "project_api";
                    options.Authority = "http://localhost:6232";//使用网关地址，会自动转发
                    options.SaveToken = true;//会把token存在header里
                });

            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                return new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
            });

            services.AddScoped<IRecommendService, TestRecommendService>()
                .AddScoped<IProjectQuery, ProjectQuery>(sp =>
                {
                    return new ProjectQuery(Configuration.GetConnectionString("MysqlProject"));
                })
                .AddScoped<IProjectRepository, ProjectRepository>(sp =>
                {
                    var context = sp.GetRequiredService<ProjectContext>();
                    return new ProjectRepository(context);
                });

            //services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMediatR(typeof(Program).Assembly);
            //如果command和handler不在一个项目，还需要引入其他项目的Assembly

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCap(options =>
            {
                options.UseEntityFramework<ProjectContext>()
                    .UseRabbitMQ("localhost")
                    .UseDashboard()
                    .UseDiscovery(c =>
                    {
                        c.DiscoveryServerHostName = "localhost";
                        c.DiscoveryServerPort = 8500;
                        c.CurrentNodeHostName = "localhost";
                        c.CurrentNodePort = 6234;
                        c.NodeId = "3";
                        c.NodeName = "project cap 3 node";
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
