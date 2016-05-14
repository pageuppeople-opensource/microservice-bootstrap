using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using static System.Threading.Thread;
using static System.Threading.Timeout;
using static System.Console;

namespace WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                    .AddEnvironmentVariables()
                                    .AddCommandLine(args)
                                    .Build();

            WriteLine("Hello World");

            // sleep indefinitely
            Sleep(Infinite);
        }
    }
}
