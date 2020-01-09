using Author.Query.New.API.GraphQL.Resolvers;
using GraphQL.Types;
using System;
using System.Linq;

namespace Author.Query.New.API.GraphQL
{
    public class GraphQLQuery : ObjectGraphType
    {
        public GraphQLQuery(IServiceProvider serviceProvider)
        {
            var type = typeof(IResolver);
            var resolversTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            foreach (var resolverType in resolversTypes)
            {
                var resolverTypeInterface = resolverType.GetInterfaces().Where(x => x != type).FirstOrDefault();
                if (resolverTypeInterface != null)
                {
                    var resolver = serviceProvider.GetService(resolverTypeInterface) as IResolver;
                    resolver.Resolve(this);
                }
            }
        }
    }
}
