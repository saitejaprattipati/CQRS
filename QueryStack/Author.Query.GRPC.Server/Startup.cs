using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Author.Query.GRPC.Server.Extensions;
using Author.Query.Persistence;
using Author.Query.Persistence.Interfaces;
using Boxed.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Author.Query.GRPC.Server.Middleware;

namespace Author.Query.GRPC.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddTransient<IArticleService, ArticleService>();
            services.AddHttpClient<IArticleService, ArticleService>();
            services.AddCosmosDBConfiguration(this.Configuration);
            services.AddAutoMapperConfiguration();
            services.AddHttpContextAccessor();
            services.AddCustomRouting();
            services.AddCustomStrictTransportSecurity();
            services.AddCustomHealthChecks();
            services.AddControllers();
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
            services.AddCustomOptions(this.Configuration);
            services.AddServerTiming();
            services.AddGraphQLResolvers();
            services.AddProjectRepositories();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseForwardedHeaders();
            app.UseFetchLocaleMiddleware();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ArticlesService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
