using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
                   .Build();
    }
}
