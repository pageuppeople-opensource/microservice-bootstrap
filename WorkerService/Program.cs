using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using static System.Threading.Thread;
using static System.Threading.Timeout;

namespace WorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Hello World!");

            await Task.Delay(1); // delete once real awaitable code is here (to prevent warnings on empty Main method)

            // sleep indefinitely
            Sleep(Infinite);
        }
    }
}
