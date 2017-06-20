using System.Reflection;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Kinesis;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkerService.EventProcessors;
using KinesisNet;
using static System.Threading.Thread;
using static System.Threading.Timeout;

namespace WorkerService
{
    public class Program
    {
        private static Environment environment;
        private static KManager kManager;

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            Log.Debug("Testing debug logging");

            environment = new Environment
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
            
            Log.Information("{@Environment}", environment);

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
            kManager.Consumer.Stop();
        }

        public static void StartListeningForEvents()
        {
            Log.Debug("Entering StartListeningForEvents");
            var kinesisStreamName = "RoleStream.LIVE";
            var kinesisWorkerId = Assembly.GetEntryAssembly().GetName().Name + "-" + environment.Env;

            Log.Information("KManager variables:");
            Log.Information($"  StreamName: {kinesisStreamName}");
            Log.Information($"  WorkerId: {kinesisWorkerId}");

            Log.Debug("Creating Dyanmo client");
            var dynamoClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig{RegionEndpoint = RegionEndpoint.GetBySystemName(environment.AwsRegion)});
            Log.Debug("Creating Kinesis client");
            var kinesisClient = new AmazonKinesisClient(new AmazonKinesisConfig{ RegionEndpoint = RegionEndpoint.GetBySystemName(environment.AwsRegion )});

            Log.Debug("Starting Kmanager");
            kManager = new KManager(dynamoClient, kinesisClient, kinesisStreamName, kinesisWorkerId);

            Log.Debug("Starting consumer");
            Task.Run(() => kManager.Consumer.Start(new RolesEventProcessor()));
        }
    }
}
