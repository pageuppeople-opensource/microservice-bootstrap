using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using static System.Threading.Thread;
using static System.Threading.Timeout;

namespace WorkerService
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Logger = logger;

            Console.WriteLine("Hello World");

            // sleep indefinitely
            Sleep(Infinite);
        }
    }
}
