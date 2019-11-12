using Author.Core.Framework;
using Author.Core.Framework.Utilities;
using Author.Query.API.Extensions;
using Author.Query.API.GraphQL;
using Author.Query.API.GraphQL.Resolvers;
using Author.Query.API.GraphQL.Types;
using Author.Query.API.Middleware;
using Author.Query.API.Options;
using Author.Query.Persistence;
using Author.Query.Persistence.Interfaces;
using Author.Query.Persistence.Mapping;
using AutoMapper;
using CorrelationId;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO.Compression;

namespace Author.Query.API
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IHostingEnvironment hostingEnvironment;
        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCorrelationIdFluent();

            #region AzureCosmosDB Configuration

            services.AddCosmosDBConfiguration(configuration);

            #endregion

            #region AutoMapper Configuration

            services.AddAutoMapperConfiguration();

            #endregion

            #region Compression Configuration

            services.AddCustomResponseCompression();

            #endregion

            #region CustomOptions

            services.AddCustomOptions(configuration);

            #endregion

            #region HttpContextAccessor

            services.AddHttpContextAccessor();

            #endregion

            services.AddCustomRouting();

            services.AddCustomStrictTransportSecurity();

            //services.AddCustomHealthChecks(Configuration);

            services.AddHealthChecks();

            ////services.AddHealthChecksUI(setupSettings: setup =>
            ////{
            ////    setup.AddHealthCheckEndpoint("Basic healthcheck", "http://localhost:58264/healthcheck");
            ////});

            //services.AddAutoMapper(typeof(Startup));
            //services.AddSingleton<CompressionOptions>();
            ////services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            ////services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            ////services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            ////services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            ////services.AddSingleton<DataLoaderDocumentListener>();
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
            services.AddMvcCore()
                    .SetCompatibilityVersion(CompatibilityVersion.Latest)
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .AddCustomJsonOptions(this.hostingEnvironment)
                    .AddCustomCors()
                    .AddCustomMvcOptions(this.hostingEnvironment);

            services.AddGraphQL(o => { o.ExposeExceptions = true; })
                    .AddGraphTypes(ServiceLifetime.Scoped)
                    .AddDataLoader();

            //services.AddSingleton<TaxatHandSchema>();
            //var sp = services.BuildServiceProvider();
            //services.AddSingleton<ISchema>(new TaxatHandSchema(new FuncDependencyResolver(type => sp.GetService(type))));

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<TaxatHandSchema>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ////app.UseResponseCompression();
            // Pass a GUID in a X-Correlation-ID HTTP header to set the HttpContext.TraceIdentifier.
            // UpdateTraceIdentifier must be false due to a bug. See https://github.com/aspnet/AspNetCore/issues/5144
            app.UseCorrelationId(new CorrelationIdOptions() { UpdateTraceIdentifier = false });
            app.UseForwardedHeaders();
            app.UseResponseCompression();
            app.UseFetchLocaleMiddleware();
            //app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHealthChecks("/healthcheck");
            ////app.UseHealthChecks("/healthcheck", new HealthCheckOptions
            ////{
            ////    Predicate = _ => true,
            ////    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            ////});

            ////app.UseHealthChecksUI();

            app.UseGraphQL<TaxatHandSchema>();
            app.UseGraphQLPlayground(options: new GraphQLPlaygroundOptions());
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
