using Author.Core.Framework;
using Author.Core.Framework.Utilities;
using Author.Query.API.GraphQL;
using Author.Query.API.GraphQL.Resolvers;
using Author.Query.API.GraphQL.Types;
using Author.Query.Persistence;
using Author.Query.Persistence.Interfaces;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            string serviceEndPoint = Configuration.GetValue<string>("CosmosDBEndpoint");
            string authKeyOrResourceToken = Configuration.GetValue<string>("CosmosDBAccessKey");
            string databaseName = Configuration.GetValue<string>("CosmosDBName");

            services.AddEntityFrameworkCosmos();
            //services.AddScoped<DbContext, TaxathandDbContext>();

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
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<DataLoaderDocumentListener>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ICountryService, CountryService>();

            services.AddScoped<CountryResultType>();
            services.AddScoped<GraphQLQuery>();
            services.AddScoped<ICountriesResolver, CountriesResolver>();
            services.AddScoped<CountryType>();

            services.AddScoped<Response>();
            services.AddScoped(typeof(ResponseGraphType<>));
            services.AddScoped(typeof(ResponseListGraphType<>));

            services.AddTransient<IAddressRepository, AddressRepository>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddGraphQL(o => { o.ExposeExceptions = true; })
                    .AddGraphTypes(ServiceLifetime.Scoped);

            //services.AddSingleton<TaxatHandSchema>();
            //var sp = services.BuildServiceProvider();
            //services.AddSingleton<ISchema>(new TaxatHandSchema(new FuncDependencyResolver(type => sp.GetService(type))));

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<TaxatHandSchema>();
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
            app.UseGraphQL<TaxatHandSchema>();
            app.UseGraphQLPlayground(options: new GraphQLPlaygroundOptions());
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
