using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkerService.KinesisNet.Model;

namespace WorkerService.KinesisNet.Interface
{
    internal interface IDynamoDB
    {
        Task Init();
        Task SaveToDatabase(string shardId, string sequenceNumber, DateTime lastUpdatedUtc);
        Task<List<KShard>> GetShards(IList<string> shardIds);
    }
}
