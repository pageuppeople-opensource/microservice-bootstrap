using System;
using System.Reflection;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Kinesis;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using SeriLog.LogSanitizingFormatter;
using WorkerService.EventProcessors;
using WorkerService.KinesisNet;
using WorkerService.KinesisNet.Interface;
using WorkerService.KinesisNet.Model;
using WorkerService.KinesisNet.Persistance;
using static System.Threading.Thread;
using static System.Threading.Timeout;
using static System.Console;

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
                System.Environment.GetEnvironmentVariable("REGION"),
                System.Environment.GetEnvironmentVariable("DC"),
                System.Environment.GetEnvironmentVariable("ENV"),
                typeof(Program)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion
            );
            
            Log.Information("{@Environment}", _environment);

            Log.Information("Starting configuration");

            var configuration = new ConfigurationBuilder()
                                    .AddEnvironmentVariables()
                                    .AddCommandLine(args)
                                    .Build();

            StartListeningForEvents();

            Log.Information("Going to sleep... zzzzz");
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
            Log.Debug("Entering StartListeningForEvents");
            var kinesisStreamName = "RoleStream.LIVE";
            var kinesisWorkerId = Assembly.GetEntryAssembly().GetName().Name + "-" + _environment.Env;

            Log.Information("KManager variables:");
            Log.Information($"  StreamName: {kinesisStreamName}");
            Log.Information($"  WorkerId: {kinesisWorkerId}");

            Log.Debug("Creating Dyanmo client");
            var dynamoClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig{RegionEndpoint = RegionEndpoint.GetBySystemName(_environment.AwsRegion)});
            Log.Debug("Creating Kinesis client");
            var kinesisClient = new AmazonKinesisClient(new AmazonKinesisConfig{ RegionEndpoint = RegionEndpoint.GetBySystemName(_environment.AwsRegion )});

            Log.Debug("Starting Kmanager");
            _kManager = new KManager(dynamoClient, kinesisClient, kinesisStreamName, kinesisWorkerId);

            Log.Debug("Starting consumer");
            Task.Run(() => _kManager.Consumer.Start(new RolesEventProcessor()));
        }
    }
}
