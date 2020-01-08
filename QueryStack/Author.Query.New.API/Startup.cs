using Author.Query.New.API.Constants;
using Author.Query.New.API.Extensions;
using Author.Query.New.API.GraphQL;
using Author.Query.New.API.Middleware;
using Boxed.AspNetCore;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Author.Query.New.API
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration, where key value pair settings are stored. See
        /// http://docs.asp.net/en/latest/fundamentals/configuration.html</param>
        /// <param name="webHostEnvironment">The environment the application is running under. This can be Development,
        /// Staging or Production by default. See http://docs.asp.net/en/latest/fundamentals/environments.html</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }


        /// <summary>
        /// Configures the services to add to the ASP.NET Core Injection of Control (IoC) container. This method gets
        /// called by the ASP.NET runtime. See
        /// http://blogs.msdn.com/b/webdev/archive/2014/06/17/dependency-injection-in-asp-net-vnext.aspx
        /// </summary>
        public virtual void ConfigureServices(IServiceCollection services) =>
            services
            .Configure<IISServerOptions>(options=>options.AllowSynchronousIO=true)
            .AddCosmosDBConfiguration(this.configuration)
            .AddAutoMapperConfiguration()
            .AddCustomResponseCompression(this.configuration)
            .AddCustomCors()
            .AddCustomOptions(this.configuration)
            .AddHttpContextAccessor()
            .AddCustomRouting()
            .AddCustomStrictTransportSecurity()
            .AddCustomHealthChecks()
            .AddServerTiming()
            .AddControllers()
                    .AddCustomJsonOptions(this.webHostEnvironment)
                    .AddCustomMvcOptions(this.configuration)
            .Services
            .AddCustomGraphQL(this.configuration, this.webHostEnvironment)
            .AddGraphQLResolvers()
            //.AddGraphQLResponse()
            .AddProjectRepositories()
            .AddProjectSchemas();

        /// <summary>
        /// Configures the application and HTTP request pipeline. Configure is called after ConfigureServices is
        /// called by the ASP.NET runtime.
        /// </summary>
        public virtual void Configure(IApplicationBuilder application) =>
            application
                .UseIf(
                    this.webHostEnvironment.IsDevelopment(),
                    x => x.UseServerTiming())
            .UseForwardedHeaders()
            .UseResponseCompression()
            .UseFetchLocaleMiddleware()
            .UseIf(
                    !this.webHostEnvironment.IsDevelopment(),
                    x => x.UseHsts())
                .UseIf(
                    this.webHostEnvironment.IsDevelopment(),
                    x => x.UseDeveloperExceptionPage())
            .UseRouting()
                .UseCors(CorsPolicyName.AllowAny)
            .UseEndpoints(
                    builder =>
                    {
                        builder
                            .MapHealthChecks("/status")
                            .RequireCors(CorsPolicyName.AllowAny);
                        builder
                            .MapHealthChecks("/status/self", new HealthCheckOptions() { Predicate = _ => false })
                            .RequireCors(CorsPolicyName.AllowAny);
                    })
            .UseWebSockets()
                // Use the GraphQL subscriptions in the specified schema and make them available at /graphql.
                .UseGraphQLWebSockets<TaxatHandSchema>()
                // Use the specified GraphQL schema and make them available at /graphql.
                .UseGraphQL<TaxatHandSchema>()
                .UseIf(
                    this.webHostEnvironment.IsDevelopment(),
                    x => x
                        // Add the GraphQL Playground UI to try out the GraphQL API at /.
                        .UseGraphQLPlayground(new GraphQLPlaygroundOptions() { Path = "/" })
                        // Add the GraphQL Voyager UI to let you navigate your GraphQL API as a spider graph at /voyager.
                        .UseGraphQLVoyager(new GraphQLVoyagerOptions() { Path = "/voyager" }));
    }
}
