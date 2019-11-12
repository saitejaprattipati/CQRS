using GraphQL.DataLoader;

namespace Author.Query.API.GraphQL.Resolvers
{
    public interface IResolver
    {
        void Resolve(GraphQLQuery graphQLQuery);

        //void Resolve(GraphQLQuery graphQLQuery, IDataLoaderContextAccessor dataLoaderContextAccessor);
    }
}
