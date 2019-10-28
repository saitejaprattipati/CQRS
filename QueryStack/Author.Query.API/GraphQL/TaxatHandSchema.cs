using GraphQL;
using GraphQL.Types;

namespace Author.Query.API.GraphQL
{
    public class TaxatHandSchema : Schema
    {
        public TaxatHandSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<GraphQLQuery>();
        }
    }
}
