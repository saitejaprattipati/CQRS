using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AuthorAdmin.Command.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options=>options.AddServerHeader = false)
                .UseStartup<Startup>();
    }
}
