using System;
using System.Reflection;
using Amazon;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkerService.KinesisNet;
using WorkerService.KinesisNet.Interface;
using WorkerService.KinesisNet.Model;
using static System.Threading.Thread;
using static System.Threading.Timeout;
using static System.Console;
using Environment = WorkerService.KinesisNet.Model.Environment;

namespace WorkerService
{
    public class Program
    {
        private static Environment _environment;
        private static KManager _kManager;

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            _environment = new Environment
            {
                AwsKey = System.Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
                AwsSecret = System.Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"),
                AwsSessionToken = System.Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN"),
                AwsRegion = System.Environment.GetEnvironmentVariable("AWS_REGION_ENDPOINT") // TODO: To verify 
            };

            Log.Information("Environment variables:");
            foreach (PropertyInfo property in _environment.GetType().GetProperties())
                Log.Information($"  {property.Name}: {property.GetValue(_environment, null)}");

            Log.Information("Starting configuration");

            var configuration = new ConfigurationBuilder()
                                    .AddEnvironmentVariables()
                                    .AddCommandLine(args)
                                    .Build();

            WriteLine("Hello World");

            StartListeningForEvents();

            // sleep indefinitely
            Sleep(Infinite);

            StopListeningToEvents();
        }

        private static void StopListeningToEvents()
        {
            _kManager.Consumer.Stop();
        }

        public static void StartListeningForEvents()
        {
            _kManager = new KManager(_environment.AwsKey, 
                _environment.AwsSecret, 
                "RoleStream.LIVE", 
                regionEndpoint, 
                _environment.AwsSessionToken, 
                "microservice-bootstrap-test");

            var result = _kManager
                        .Consumer
                        .Start(new RecordProcessor());

           
        }
    }
}
