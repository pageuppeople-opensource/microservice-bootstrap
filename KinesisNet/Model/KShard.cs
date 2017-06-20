using System;
using System.Linq;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;

namespace KinesisNet.Model
{
    internal class KShard
    {
        public string ShardId { get; private set; }
        public string SequenceNumber { get; private set; }
        public string StreamName { get; private set; }
        public DateTime LastUpdateUtc { get; private set; }

        public string ShardIterator { get; private set; }

        public ShardIteratorType ShardIteratorType { get; private set; }

        public KShard(string shardId, string streamName) : this(shardId, streamName, Amazon.Kinesis.ShardIteratorType.TRIM_HORIZON, null)
        {
        }

        public KShard(string shardId, string streamName, ShardIteratorType shardIteratorType, string sequenceNumber)
        {
            ShardId = shardId;
            StreamName = streamName;
            ShardIteratorType = shardIteratorType;
            SequenceNumber = sequenceNumber;

            LastUpdateUtc = DateTime.UtcNow;
        }

        public void UpdateShardInformation(GetRecordsResponse recordsResponse)
        {
            SetNextShardIterator(recordsResponse.NextShardIterator);

            if (recordsResponse.Records.Count > 0)
            {
                SequenceNumber = recordsResponse.Records.LastOrDefault().SequenceNumber;
            }

            LastUpdateUtc = DateTime.UtcNow;
        }

        public void SetNextShardIterator(string shardIterator)
        {
            ShardIterator = shardIterator;
        }
    }
}
