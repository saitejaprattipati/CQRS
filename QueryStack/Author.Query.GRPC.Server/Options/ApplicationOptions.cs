namespace Author.Query.GRPC.Server.Options
{
    using Author.Core.Framework;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
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
        public AppSettings AppSettings { get; set; }

        [Required]
        public KestrelServerOptions Kestrel { get; set; }
    }
}
