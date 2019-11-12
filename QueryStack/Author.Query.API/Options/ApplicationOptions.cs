namespace Author.Query.API.Options
{
    using Author.Core.Framework;
    using global::GraphQL.Server;
    using Microsoft.AspNetCore.Builder;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// All options for the application.
    /// </summary>
    public class ApplicationOptions
    {
        [Required]
        public CacheProfileOptions CacheProfiles { get; set; }

        public CompressionOptions Compression { get; set; }

        [Required]
        public ForwardedHeadersOptions ForwardedHeaders { get; set; }

        [Required]
        public GraphQLOptions GraphQL { get; set; }

        [Required]
        public AppSettings AppSettings { get; set; }

        //[Required]
        //public KestrelServerOptions Kestrel { get; set; }
    }
}
