using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace WorkerService.KinesisNet.Model
{
    public class Environment
    {
        public Environment(string awsKey, string awsSecret, string awsSessionToken, string awsRegion, string dc, string env)
        {
            if (string.IsNullOrWhiteSpace(awsSessionToken))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(awsSessionToken));
            if (string.IsNullOrWhiteSpace(awsRegion))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(awsRegion));
            if (string.IsNullOrWhiteSpace(dc))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dc));
            if (string.IsNullOrWhiteSpace(env))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(env));

            Public = new PublicEnvironment(awsKey, awsRegion, dc, env);
            Private = new PrivateEnvionment(awsSecret, awsSessionToken);
        }

        public PrivateEnvionment Private { get; }

        public PublicEnvironment Public { get; }

        public class PrivateEnvionment
        {
            public PrivateEnvionment(string awsSecret, string awsSessionToken)
            {
                if (string.IsNullOrWhiteSpace(awsSecret))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(awsSecret));
                if (string.IsNullOrWhiteSpace(awsSessionToken))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(awsSessionToken));

                AwsSecret = awsSecret;
                AwsSessionToken = awsSessionToken;
            }

            public string AwsSecret { get; set; }
            public string AwsSessionToken { get; set; }
        }

        public class PublicEnvironment
        {
            public PublicEnvironment(string awsKey, string awsRegion, string dc, string env)
            {
                if (string.IsNullOrWhiteSpace(awsKey))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(awsKey));
                if (string.IsNullOrWhiteSpace(awsRegion))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(awsRegion));
                if (string.IsNullOrWhiteSpace(dc))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(dc));
                if (string.IsNullOrWhiteSpace(env))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(env));

                AwsKey = awsKey;
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

}
