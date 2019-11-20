using Author.Query.API.Extensions;
using Author.Query.API.GraphQL;
using Author.Query.API.Middleware;
using Boxed.AspNetCore;
using CorrelationId;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Author.Query.API
{
    public class Startup : IStartup
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
        public IServiceProvider ConfigureServices(IServiceCollection services) =>
            services
                .AddCorrelationIdFluent()
                .AddCosmosDBConfiguration(configuration)
                .AddAutoMapperConfiguration()
                .AddCustomResponseCompression()
                .AddCustomOptions(configuration)
                .AddHttpContextAccessor()
                .AddCustomRouting()
                .AddCustomStrictTransportSecurity()
                .AddCustomHealthChecks()
                .AddMvcCore()
                    .SetCompatibilityVersion(CompatibilityVersion.Latest)
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .AddCustomJsonOptions(this.hostingEnvironment)
                    .AddCustomCors()
                    .AddCustomMvcOptions(this.hostingEnvironment)
                .Services
                .AddCustomGraphQL(this.hostingEnvironment)
                .AddProjectRepositories()
                .AddGraphQLResolvers()
                .AddGraphQLResponse()
                .AddProjectSchemas()
                .BuildServiceProvider();

        /// <summary>
        /// Configures the application and HTTP request pipeline. Configure is called after ConfigureServices is
        /// called by the ASP.NET runtime.
        /// </summary>
        public void Configure(IApplicationBuilder application) =>
            application
                // Pass a GUID in a X-Correlation-ID HTTP header to set the HttpContext.TraceIdentifier.
                // UpdateTraceIdentifier must be false due to a bug. See https://github.com/aspnet/AspNetCore/issues/5144
                .UseCorrelationId(new CorrelationIdOptions() { UpdateTraceIdentifier = false })
                .UseForwardedHeaders()
                .UseResponseCompression()
                .UseFetchLocaleMiddleware()
                .UseCors(CorsPolicyName.AllowAny)
                .UseIf(
                    !this.hostingEnvironment.IsDevelopment(),
                    x => x.UseHsts())
                .UseIf(
                    this.hostingEnvironment.IsDevelopment(),
                    x => x.UseDeveloperErrorPages())
                .UseHealthChecks("/healthcheck")
                // Use the specified GraphQL schema and make them available at /graphql.
                .UseGraphQL<TaxatHandSchema>()
                .UseHttpsRedirection()
                .UseIf(
                    this.hostingEnvironment.IsDevelopment(),
                    x => x
                        // Add the GraphQL Playground UI to try out the GraphQL API at /.
                        .UseGraphQLPlayground(new GraphQLPlaygroundOptions() { Path = "/" })
                        // Add the GraphQL Voyager UI to let you navigate your GraphQL API as a spider graph at /voyager.
                        .UseGraphQLVoyager(new GraphQLVoyagerOptions() { Path = "/voyager" }));
    }
}
