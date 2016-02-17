using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading;

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

            Console.WriteLine("Hello World");

            // sleep indefinitely
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
