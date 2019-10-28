namespace Author.Query.API.GraphQL.Errors
{
    public class NotFoundError : GraphQLError
    {
        public NotFoundError(string id) : base(nameof(NotFoundError), $"Resource '{id}' not found.")
        {
        }
    }
}
