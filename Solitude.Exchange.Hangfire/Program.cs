using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Solitude.Exchange.Core;

namespace Solitude.Exchange.Hangfire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(BaseCore.Configuration)
            .UseStartup<Startup>();
        }

    }
}
