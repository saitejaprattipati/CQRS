using GraphQL.Instrumentation;
using GraphQL.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Author.Query.API.GraphQL.Middleware
{
    public class InstrumentFieldsMiddleware
    {
        public async Task<object> Resolve(ResolveFieldContext context, FieldMiddlewareDelegate next)
        {
            var metadata = new Dictionary<string, object>
            {
                { "typeName", context.ParentType.Name },
                { "fieldName", context.FieldName },
                { "path", context.Path },
                { "arguments", context.Arguments },
            };
            var path = $"{context.ParentType.Name}.{context.FieldName}";

            using (context.Metrics.Subject("field", path, metadata))
            {
                return await next(context).ConfigureAwait(false);
            }
        }
    }
}
