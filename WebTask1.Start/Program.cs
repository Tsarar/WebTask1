using System.IO;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebTask1.Start
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
        
        public static IWebHost BuildWebHost(string[] args) =>
                       WebHost.CreateDefaultBuilder()
                              .UseStartup<Startup>()
                              .UseConfiguration(new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json")
                                  .Build())
                              .Build();
    }
}

