using System;
using System.Collections.Generic;
using Amazon.Kinesis.Model;

namespace WorkerService.KinesisNet.Interface
{
    public interface IRecordProcessor
    {
        void Process(string shardId, string sequenceNumber, DateTime lastUpdateUtc, IList<Record> records, Action<string, string, DateTime> saveCheckpoint);
    }
}
