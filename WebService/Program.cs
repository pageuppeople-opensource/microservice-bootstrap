using System.IO;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace WebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            Log.Information("Creating host");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://*:4000")
                .Build();

            Log.Information("Starting host");

            host.Run();
        }
    }
}
