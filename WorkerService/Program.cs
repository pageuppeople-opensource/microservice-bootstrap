using System;
using System.Reflection;
using Amazon;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using SeriLog.LogSanitizingFormatter;
using WorkerService.EventProcessors;
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

            //new LoggerConfiguration().WriteTo.Co
            _environment = new Environment
            (
                System.Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
                System.Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"),
                System.Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN"),
                System.Environment.GetEnvironmentVariable("REGION"),
                System.Environment.GetEnvironmentVariable("DC"),
                System.Environment.GetEnvironmentVariable("Env")
            );
            
            Log.Information("{@Environment}", _environment.Public);

            Log.Information("Starting configuration");

            var configuration = new ConfigurationBuilder()
                                    .AddEnvironmentVariables()
                                    .AddCommandLine(args)
                                    .Build();

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
            var kinesisStreamName = "RoleStream.LIVE";
            var kinesisWorkerId = Assembly.GetEntryAssembly().GetName().Name;

            Log.Information("KManager variables:");
            Log.Information($"  StreamName: {kinesisStreamName}");
            Log.Information($"  WorkerId: {kinesisWorkerId}");

            _kManager = new KManager(_environment.Public.AwsKey, 
                _environment.Private.AwsSecret, 
                kinesisStreamName, 
                RegionEndpoint.GetBySystemName(_environment.Public.AwsRegion), 
                _environment.Private.AwsSessionToken, 
                kinesisWorkerId);

            _kManager.Consumer.Start(new RolesEventProcessor());
        }
    }
}
