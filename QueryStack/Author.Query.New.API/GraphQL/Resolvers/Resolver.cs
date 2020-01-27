using Author.Query.New.API.GraphQL.Errors;

namespace Author.Query.New.API.GraphQL.Resolvers
{
    public class Resolver
    {
        public Response Response(object data)
        {
            return new Response(data);
        }

        public Response Error(GraphQLError error)
        {
            return new Response(error.StatusCode, error.ErrorMessage);
        }

        public Response AccessDeniedError()
        {
            var error = new AccessDeniedError();
            return new Response(error.StatusCode, error.ErrorMessage);
        }

        public Response NotFoundError(string id)
        {
            var error = new NotFoundError(id);
            return new Response(error.StatusCode, error.ErrorMessage);
        }
    }
}
