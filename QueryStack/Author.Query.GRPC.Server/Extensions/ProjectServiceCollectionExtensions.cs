using Author.Core.Framework.Utilities;
using Author.Query.Persistence;
using Author.Query.Persistence.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Author.Query.GRPC.Server.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods add project services.
    /// </summary>
    /// <remarks>
    /// AddSingleton - Only one instance is ever created and returned.
    /// AddScoped - A new instance is created and returned for each request/response cycle.
    /// AddTransient - A new instance is created and returned each time.
    /// </remarks>
    public static class ProjectServiceCollectionExtensions
    {
        /// <summary>
        /// Add project data repositories.
        /// </summary>
        //public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
        //    services
        //        .AddScoped<IUtilityService, UtilityService>()
        //        .AddScoped<ICommonService, CommonService>()
        //        .AddScoped<IImageService, ImageService>()
        //        .AddScoped(typeof(ICacheService<,>), typeof(CacheService<,>))
        //        .AddScoped<ICountryService, CountryService>()
        //        .AddScoped<ICountryGroupService, CountryGroupService>()
        //        .AddScoped<IDisclaimerService, DisclaimerService>()
        //        .AddScoped<ITaxTagsService, TaxTagsService>()
        //        .AddScoped<IResourceGroupService, ResourceGroupService>();


        //public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
        //    services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(CommonService)))
        //    //services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetExecutingAssembly())
        //            .Where(c => c.Name.EndsWith("Persistence"))
        //            .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

        //public static IServiceCollection AddGraphQLResolvers(this IServiceCollection services) =>
        //services
        //    .AddScoped<ICountriesResolver, CountriesResolver>()
        //    .AddScoped<ICountryGroupsResolver, CountryGroupsResolver>()
        //    .AddScoped<IDisclaimerResolver, DisclaimerResolver>()
        //    .AddScoped<ITaxTagGroupsResolver, TaxTagGroupsResolver>()
        //    .AddScoped<IResourceGroupResolver, ResourceGroupResolver>();

        public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
            services.Scan(scan => scan
                    .FromAssembliesOf(typeof(ICommonService), typeof(IUtilityService))
                    .AddClasses(classes => classes.InNamespaces(new List<string> { "Author.Query.Persistence", "Author.Query.Persistence.Interfaces", "Author.Core.Framework.Utilities" }))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                );


        public static IServiceCollection AddGraphQLResolvers(this IServiceCollection services) =>
           services.Scan(scan => scan
                   .FromCallingAssembly()
                   .AddClasses(classes => classes.InNamespaces("Author.Query.New.API.GraphQL.Resolvers"))
                   .AsImplementedInterfaces()
                   .WithScopedLifetime()
               );
    }
}
