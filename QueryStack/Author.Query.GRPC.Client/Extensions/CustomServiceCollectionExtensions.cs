using Author.Core.Framework;
using Author.Query.Persistence;
using Author.Query.Persistence.Mapping;
using AutoMapper;
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




using Author.Core.Framework.Utilities;
using Author.Query.Persistence.Interfaces;
using System.Collections.Generic;




namespace Author.Query.GRPC.Client.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods which extend ASP.NET Core services.
    /// </summary>
    public static class CustomServiceCollectionExtensions
    {

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
                .AddDbContextPool<TaxathandDbContext>(
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




        //public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
        //   services.Scan(scan => scan
        //           .FromAssembliesOf(typeof(ICommonService), typeof(IUtilityService))
        //           .AddClasses(classes => classes.InNamespaces(new List<string> { "Author.Query.Persistence", "Author.Query.Persistence.Interfaces", "Author.Core.Framework.Utilities" }))
        //           .AsImplementedInterfaces()
        //           .WithScopedLifetime()
        //       );
    }
}
