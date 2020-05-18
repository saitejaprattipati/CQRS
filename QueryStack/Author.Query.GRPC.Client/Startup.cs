using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Author.Query.GRPC.Client.Services;
using Author.Query.GRPC.Client.Extensions;
using Author.Query.GRPC.Client.Config;

namespace Author.Query.GRPC.Client
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
            services.AddHttpClient<IArticleService, ArticleService>();
            services.AddOptions();
            services.Configure<UrlsConfig>(Configuration.GetSection("urls"));
            //  services.AddCosmosDBConfiguration(this.Configuration);
            //   services.AddAutoMapperConfiguration();
            //  services.AddHttpContextAccessor();
            // services.AddCustomRouting();
            // services.AddCustomStrictTransportSecurity();
            //  services.AddCustomHealthChecks();
            services.AddControllers();
            //.Services
            //.AddProjectRepositories();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
