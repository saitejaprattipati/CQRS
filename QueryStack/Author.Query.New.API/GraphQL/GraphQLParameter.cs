using Newtonsoft.Json.Linq;

namespace Author.Query.New.API.GraphQL
{
    public class GraphQLParameter
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }

        public string Query { get; set; }
        public JObject Variables { get; set; } //https://github.com/graphql-dotnet/graphql-dotnet/issues/389
    }
}
