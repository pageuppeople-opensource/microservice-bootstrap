using System;

namespace WorkerService
{
    public class Environment
    {
        public Environment(string awsRegion, string dc, string env, string version)
        {
            if (string.IsNullOrWhiteSpace(awsRegion))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(awsRegion));
            if (string.IsNullOrWhiteSpace(dc))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dc));
            if (string.IsNullOrWhiteSpace(env))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(env));
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(version));

            AwsRegion = awsRegion;
            Dc = dc;
            Env = env;
            Version = version;
        }

        public string AwsKey { get; }
        public string AwsRegion { get;  }
        public string Dc { get;  }
        public string Env { get; }
        public string Version { get; }
    }

}
