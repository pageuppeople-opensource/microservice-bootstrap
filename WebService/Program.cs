using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://*:4000")
                .Build();

            host.Run();
        }
    }
}
