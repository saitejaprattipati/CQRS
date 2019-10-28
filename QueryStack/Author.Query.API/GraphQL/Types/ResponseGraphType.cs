using GraphQL.Types;

namespace Author.Query.API.GraphQL.Types
{
    public class ResponseGraphType<TGraphType> : ObjectGraphType<Response> where TGraphType : GraphType
    {
        public ResponseGraphType()
        {
            Name = $"Response{typeof(TGraphType).Name}";

            Field(x => x.StatusCode, nullable: true).Description("Status code of the request.");
            Field(x => x.ErrorMessage, nullable: true).Description("Error message if requests fails.");

            Field<TGraphType>(
                "data",
                "Data returned by query.",
                resolve: context => context.Source.Data
            );
        }
    }
}
