using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Author.Query.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;

namespace Author.Query.API
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
            string serviceEndPoint =
        this.Configuration.GetValue<string>("CosmosDBEndpoint");
            string authKeyOrResourceToken =
                this.Configuration.GetValue<string>("CosmosDBAccessKey");
            string databaseName =
                this.Configuration.GetValue<string>("CosmosDBName");
            services
               .AddEntityFrameworkCosmos();
            services
                .AddDbContext<TaxathandDbContext>(
                         options =>
            options.UseCosmos(
                serviceEndPoint,
                authKeyOrResourceToken,
                databaseName,
                contextOptions =>
                {
                    contextOptions.ExecutionStrategy(d => new CosmosExecutionStrategy(d));
                }
            )
        );
            services.AddTransient<IAddressRepository, AddressRepository>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
