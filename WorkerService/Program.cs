using System.Threading;
using System.Reflection;
using Amazon;
using Microsoft.Extensions.Configuration;
using Serilog;
using WorkerService.EventProcessors;
using WorkerService.KinesisNet;
using static System.Threading.Thread;
using static System.Threading.Timeout;

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
            ToggleHealth();

            // sleep indefinitely
            Sleep(Infinite);

            StopListeningToEvents();

        }

        private static void ToggleHealth()
        {
            TimerCallback log10Errors = (_) =>
            {
                for (var i = 0; i < 10; i++)
                    Log.Error("Demo error occured!");
            };

            new Timer(log10Errors, null, 0, 60000);
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

            _kManager = new KManager(
                _environment.Public.AwsKey, 
                _environment.Private.AwsSecret, 
                kinesisStreamName, 
                RegionEndpoint.GetBySystemName(_environment.Public.AwsRegion), 
                _environment.Private.AwsSessionToken, 
                kinesisWorkerId);

            _kManager.Consumer.Start(new RolesEventProcessor());
        }
    }
}
