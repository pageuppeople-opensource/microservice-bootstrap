using System;

namespace WorkerService
{
    public class Environment
    {
        public Environment(string awsRegion, string dc, string env)
        {
            if (string.IsNullOrWhiteSpace(awsRegion))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(awsRegion));
            if (string.IsNullOrWhiteSpace(dc))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dc));
            if (string.IsNullOrWhiteSpace(env))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(env));

            AwsRegion = awsRegion;
            Dc = dc;
            Env = env;
        }

        public string AwsKey { get; set; }
        public string AwsRegion { get; set; }
        public string Dc { get; set; }
        public string Env { get; set; }
    }

}
