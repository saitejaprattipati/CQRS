using Author.Core.Framework;
using Author.Query.New.API.Constants;
using Author.Query.New.API.Options;
using Author.Query.Persistence;
using Author.Query.Persistence.Mapping;
using AutoMapper;
using Boxed.AspNetCore;
using CorrelationId;
using GraphQL;
using GraphQL.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Linq;

namespace Author.Query.New.API.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods which extend ASP.NET Core services.
    /// </summary>
    public static class CustomServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIdFluent(this IServiceCollection services)
        {
            services.AddCorrelationId();
            return services;
        }

        /// <summary>
        /// Adds dynamic response compression to enable GZIP compression of responses. This is turned off for HTTPS
        /// requests by default to avoid the BREACH security vulnerability.
        /// </summary>
        public static IServiceCollection AddCustomResponseCompression(this IServiceCollection services, IConfiguration configuration) =>
            services
                .Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
                .Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
                .AddResponseCompression(
                    options =>
                    {
                        // Add additional MIME types (other than the built in defaults) to enable GZIP compression for.
                        var customMimeTypes = configuration
                            .GetSection(nameof(ApplicationOptions.Compression))
                            .Get<CompressionOptions>()
                            .MimeTypes ?? Enumerable.Empty<string>();
                        options.MimeTypes = customMimeTypes.Concat(ResponseCompressionDefaults.MimeTypes);

                        options.Providers.Add<BrotliCompressionProvider>();
                        options.Providers.Add<GzipCompressionProvider>();
                        //options.EnableForHttps = true;
                    });

        /// <summary>
        /// Add Automapper configuration
        /// </summary>
        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }

        /// <summary>
        /// Add CosmosDB configuration
        /// </summary>
        public static IServiceCollection AddCosmosDBConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            string serviceEndPoint = configuration.GetValue<string>("CosmosDBEndpoint");
            string authKeyOrResourceToken = configuration.GetValue<string>("CosmosDBAccessKey");
            string databaseName = configuration.GetValue<string>("CosmosDBName");

            services.AddEntityFrameworkCosmos();
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

            return services;
        }

        /// <summary>
        /// Configures the settings by binding the contents of the appsettings.json file to the specified Plain Old CLR
        /// Objects (POCO) and adding <see cref="IOptions{T}"/> objects to the services collection.
        /// </summary>
        public static IServiceCollection AddCustomOptions(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services
                // ConfigureAndValidateSingleton registers IOptions<T> and also T as a singleton to the services collection.
                .ConfigureAndValidateSingleton<ApplicationOptions>(configuration)
                .ConfigureAndValidateSingleton<CacheProfileOptions>(configuration.GetSection(nameof(ApplicationOptions.CacheProfiles)))
                .ConfigureAndValidateSingleton<AppSettings>(configuration.GetSection(nameof(ApplicationOptions.AppSettings)))
                .ConfigureAndValidateSingleton<CompressionOptions>(configuration.GetSection(nameof(ApplicationOptions.Compression)))
                .ConfigureAndValidateSingleton<ForwardedHeadersOptions>(configuration.GetSection(nameof(ApplicationOptions.ForwardedHeaders)))
                .ConfigureAndValidateSingleton<GraphQLOptions>(configuration.GetSection(nameof(ApplicationOptions.GraphQL)))
                .ConfigureAndValidateSingleton<KestrelServerOptions>(configuration.GetSection(nameof(ApplicationOptions.Kestrel)));

        /// <summary>
        /// Add custom routing settings which determines how URL's are generated.
        /// </summary>
        public static IServiceCollection AddCustomRouting(this IServiceCollection services) =>
            services.AddRouting(options => options.LowercaseUrls = true);

        /// <summary>
        /// Adds the Strict-Transport-Security HTTP header to responses. This HTTP header is only relevant if you are
        /// using TLS. It ensures that content is loaded over HTTPS and refuses to connect in case of certificate
        /// errors and warnings.
        /// See https://developer.mozilla.org/en-US/docs/Web/Security/HTTP_strict_transport_security and
        /// http://www.troyhunt.com/2015/06/understanding-http-strict-transport.html
        /// Note: Including subdomains and a minimum maxage of 18 weeks is required for preloading.
        /// Note: You can refer to the following article to clear the HSTS cache in your browser:
        /// http://classically.me/blogs/how-clear-hsts-settings-major-browsers
        /// </summary>
        public static IServiceCollection AddCustomStrictTransportSecurity(this IServiceCollection services) =>
            services
                .AddHsts(
                    options =>
                    {
                        // Preload the HSTS HTTP header for better security. See https://hstspreload.org/
                        // options.IncludeSubDomains = true;
                        // options.MaxAge = TimeSpan.FromSeconds(31536000); // 1 Year
                        // options.Preload = true;
                    });

        //public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var accountName = configuration.GetValue<string>("AzureStorageAccountName");
        //    var accountKey = configuration.GetValue<string>("AzureStorageAccountKey");

        //    var cosmosDBServiceEndPoint = configuration.GetValue<string>("CosmosDBEndpoint");
        //    var cosmosDBAuthKeyOrResourceToken = configuration.GetValue<string>("CosmosDBAccessKey");
        //    var cosmosDBConnectionString = $"AccountEndpoint={cosmosDBServiceEndPoint};AccountKey={cosmosDBAuthKeyOrResourceToken};";

        //    var hcBuilder = services.AddHealthChecks();

        //    hcBuilder
        //        .AddCheck("self", () => HealthCheckResult.Healthy())
        //        .AddCosmosDb(connectionString: cosmosDBConnectionString,
        //                     name: "CosmosDB-check",
        //                     failureStatus: HealthStatus.Degraded,
        //                     tags: new string[] { "cosmosdb" });

        //    services.AddHealthChecksUI(setupSettings: setup =>
        //    {
        //        setup.AddHealthCheckEndpoint("Basic healthcheck", "http://localhost:62665/healthcheck");
        //    });

        //    return services;
        //}

        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services) =>
        services
            .AddHealthChecks()
            // Add health checks for external dependencies here. See https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
            .Services;

        public static IServiceCollection AddCustomGraphQL(this IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment) =>
            services
                // Add a way for GraphQL.NET to resolve types.
                .AddScoped<IDependencyResolver, GraphQLDependencyResolver>()
                .AddGraphQL(
                    options =>
                    {
                        //var configuration = services
                        //    .BuildServiceProvider()
                        //    .GetRequiredService<IOptions<GraphQLOptions>>()
                        //    .Value;
                        var graphQLOptions = configuration
                            .GetSection(nameof(ApplicationOptions.GraphQL))
                            .Get<GraphQLOptions>();

                        // Set some limits for security, read from configuration.
                        options.ComplexityConfiguration = graphQLOptions.ComplexityConfiguration;

                        // Enable GraphQL metrics to be output in the response, read from configuration.
                        //options.EnableMetrics = graphQLOptions.EnableMetrics;
                        // Show stack traces in exceptions. Don't turn this on in production.
                        options.ExposeExceptions = webHostEnvironment.IsDevelopment();
                    })
                // Adds all graph types in the current assembly with a singleton lifetime.
                .AddGraphTypes(ServiceLifetime.Scoped)
                // Adds ConnectionType<T>, EdgeType<T> and PageInfoType.
                ////.AddRelayGraphTypes()
                // Add a user context from the HttpContext and make it available in field resolvers.
                //.AddUserContextBuilder<GraphQLUserContextBuilder>()
                // Add GraphQL data loader to reduce the number of calls to our repository.
                .AddDataLoader()
                // Add WebSockets support for subscriptions.
                .AddWebSockets()
                .Services;
        //.AddTransient(typeof(IGraphQLExecuter<>), typeof(InstrumentingGraphQLExecutor<>));

        /// <summary>
        /// Add cross-origin resource sharing (CORS) services and configures named CORS policies. See
        /// https://docs.asp.net/en/latest/security/cors.html
        /// </summary>
        public static IServiceCollection AddCustomCors(this IServiceCollection services) =>
            services.AddCors(
                options =>
                    // Create named CORS policies here which you can consume using application.UseCors("PolicyName")
                    // or a [EnableCors("PolicyName")] attribute on your controller or action.
                    options.AddPolicy(
                        CorsPolicyName.AllowAny,
                        x => x
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()));
    }
}
