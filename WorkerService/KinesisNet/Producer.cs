using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using WorkerService.KinesisNet.Interface;

namespace WorkerService.KinesisNet
{
    internal class Producer : IProducer
    {
        private readonly AmazonKinesisClient _client;
        private readonly IUtilities _utilities;

        public Producer(AmazonKinesisClient client, IUtilities utilities)
        {
            _client = client;
            _utilities = utilities;
        }

        public PutRecordResponse PutRecord(string record, string partitionKey = null)
        {
            if (string.IsNullOrEmpty(_utilities.StreamName))
            {
                throw new Exception("You must set a stream name before you can put a record");
            }

            var bytes = Encoding.UTF8.GetBytes(record);

            return PutRecord(bytes, partitionKey);
        }

        public PutRecordResponse PutRecord(byte[] data, string partitionKey = null)
        {
            if (string.IsNullOrEmpty(_utilities.StreamName))
            {
                throw new Exception("You must set a stream name before you can put a record");
            }

            using (var ms = new MemoryStream(data))
            {
                var requestRecord = new PutRecordRequest()
                {
                    StreamName = _utilities.StreamName,
                    Data = ms,
                    PartitionKey = partitionKey ?? Guid.NewGuid().ToString()
                };

                return AsyncHelper.RunSync(() => _client.PutRecordAsync(requestRecord));
            }
        }

        public async Task<PutRecordResponse> PutRecordAsync(string record, string partitionKey = null)
        {
            if (string.IsNullOrEmpty(_utilities.StreamName))
            {
                throw new Exception("You must set a stream name before you can put a record");
            }

            var bytes = Encoding.UTF8.GetBytes(record);

            return await PutRecordAsync(bytes, partitionKey);
        }

        public async Task<PutRecordResponse> PutRecordAsync(byte[] data, string partitionKey = null)
        {
            if (string.IsNullOrEmpty(_utilities.StreamName))
            {
                throw new Exception("You must set a stream name before you can put a record");
            }

            using (var ms = new MemoryStream(data))
            {
                var requestRecord = new PutRecordRequest()
                {
                    StreamName = _utilities.StreamName,
                    Data = ms,
                    PartitionKey = partitionKey ?? Guid.NewGuid().ToString()
                };

                return await _client.PutRecordAsync(requestRecord, CancellationToken.None);
            }
        }
    }
}
