using Author.Core.Framework.Utilities;
using Author.Query.New.API.GraphQL;
using Author.Query.New.API.GraphQL.Resolvers;
using Author.Query.Persistence;
using Author.Query.Persistence.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using System.Reflection;

namespace Author.Query.New.API.Extensions
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
        public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
            services
                .AddScoped<IUtilityService, UtilityService>()
                .AddScoped<ICommonService, CommonService>()
                .AddScoped<IImageService, ImageService>()
                .AddScoped(typeof(ICacheService<,>), typeof(CacheService<,>))
                .AddScoped<ICountryService, CountryService>()
                .AddScoped<ICountryGroupService, CountryGroupService>()
                .AddScoped<IDisclaimerService, DisclaimerService>()
                .AddScoped<ITaxTagsService, TaxTagsService>();

        //public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
        //    services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(CommonService)))
        //    //services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetExecutingAssembly())
        //            .Where(c => c.Name.EndsWith("Persistence"))
        //            .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

        public static IServiceCollection AddGraphQLResolvers(this IServiceCollection services) =>
        services
            .AddScoped<ICountriesResolver, CountriesResolver>()
            .AddScoped<ICountryGroupsResolver, CountryGroupsResolver>()
            .AddScoped<IDisclaimerResolver, DisclaimerResolver>()
            .AddScoped<ITaxTagGroupsResolver, TaxTagGroupsResolver>();

        /// <summary>
        /// Add project GraphQL schema and web socket types.
        /// </summary>
        public static IServiceCollection AddProjectSchemas(this IServiceCollection services) =>
            services
                .AddScoped<TaxatHandSchema>();

        public static IServiceCollection AddGraphQLResponse(this IServiceCollection services) =>
            services
                .AddScoped<Response>();
    }
}
