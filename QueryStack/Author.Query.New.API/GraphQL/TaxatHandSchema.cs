using GraphQL;
using GraphQL.Types;

namespace Author.Query.New.API.GraphQL
{
    public class TaxatHandSchema : Schema
    {
        public TaxatHandSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<GraphQLQuery>();
        }
    }
}
