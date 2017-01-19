using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Kinesis.Model;
using WorkerService.KinesisNet.Interface;

namespace WorkerService.KinesisNet.Model
{
    public class RecordProcessor : IRecordProcessor
    {
        public void Process(string shardId, string sequenceNumber, DateTime lastUpdateUtc, IList<Record> records, Action<string, string, DateTime> saveCheckpoint)
        {
            foreach (var record in records)
            {
                var msg = Encoding.UTF8.GetString(record.Data.ToArray());
                Console.WriteLine("ShardId: {0}, Data: {1}", shardId, msg);
            }

            //save the checkpoint to dynamodb to say that we've successfully processed our records 
            saveCheckpoint(arg1: shardId, arg2: sequenceNumber, arg3: lastUpdateUtc);
        }
    }
}