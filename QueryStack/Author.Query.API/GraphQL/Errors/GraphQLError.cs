namespace Author.Query.API.GraphQL.Errors
{
    public class GraphQLError
    {
        public string StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        protected GraphQLError(string statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
