using GraphQL.DataLoader;

namespace Author.Query.New.API.GraphQL.Resolvers
{
    public interface IResolver
    {
        void Resolve(GraphQLQuery graphQLQuery);

        //void Resolve(GraphQLQuery graphQLQuery, IDataLoaderContextAccessor dataLoaderContextAccessor);
    }
}
