using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkerService.KinesisNet.Model
{
    public class Environment
    {
        public string AwsKey { get; set; }
        public string AwsSecret { get; set; }
        public string AwsSessionToken { get; set; }
        public string AwsRegion { get; set; }
    }
}
