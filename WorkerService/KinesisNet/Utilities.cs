using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Serilog;
using WorkerService.KinesisNet.Interface;

namespace WorkerService.KinesisNet
{
    internal class Utilities : IUtilities
    {
        private readonly IAmazonKinesis _client;

        private string _streamName;
        private string _workerId;
        private ILogger _logger;

        private int _readCapacityUnits;
        private int _writeCapacityUnits;

        private static ConcurrentDictionary<string, string> _workingConsumer; 

        public Utilities(IAmazonKinesis client, string workerId)
        {
            _client = client;

            _readCapacityUnits = 1;
            _writeCapacityUnits = 1;

            _workerId = workerId;

            if (_workingConsumer == null)
            {
                _workingConsumer = new ConcurrentDictionary<string, string>();
            }
        }

        public string WorkerId
        {
            get { return _workerId; }
        }

        public string StreamName
        {
            get { return _streamName; }
        }

        public int DynamoReadCapacityUnits
        {
            get { return _readCapacityUnits; }
        }

        public int DynamoWriteCapacityUnits
        {
            get { return _writeCapacityUnits; }
        }

        public IUtilities SetLogConfiguration(LoggerConfiguration configuration)
        {
            _logger = configuration.CreateLogger();

            Serilog.Log.Logger = _logger;

            return this;
        }

        public ILogger Log
        {
            get { return _logger; }
        }

        public SplitShardResponse SplitShard(Shard shard)
        {
            var startingHashKey = BigInteger.Parse(shard.HashKeyRange.StartingHashKey);
            var endingHashKey = BigInteger.Parse(shard.HashKeyRange.EndingHashKey);
            var newStartingHashKey = BigInteger.Divide(BigInteger.Add(startingHashKey, endingHashKey), new BigInteger(2));

            var splitShardRequest = new SplitShardRequest { StreamName = _streamName, ShardToSplit = shard.ShardId, NewStartingHashKey = newStartingHashKey.ToString() };

            var response = AsyncHelper.RunSync(() => _client.SplitShardAsync(splitShardRequest));

            return response;
        }

        public MergeShardsResponse MergeShards(string leftShard, string rightShard)
        {
            var mergeShardRequest = new MergeShardsRequest
            {
                ShardToMerge = leftShard,
                AdjacentShardToMerge = rightShard,
                StreamName = _streamName
            };
            
            var response = AsyncHelper.RunSync(() => _client.MergeShardsAsync(mergeShardRequest));

            return response;
        }

        public DescribeStreamResponse GetStreamResponse(string streamName = null)
        {
            if (string.IsNullOrEmpty(streamName) && string.IsNullOrEmpty(_streamName))
            {
                throw new Exception("Please specify a stream name to get the stream response.");
            }

            var request = new DescribeStreamRequest() { StreamName = streamName ?? _streamName };

            return AsyncHelper.RunSync(() => _client.DescribeStreamAsync(request));
        }

        public async Task<DescribeStreamResponse> GetStreamResponseAsync(string streamName = null)
        {
            if (string.IsNullOrEmpty(streamName) && string.IsNullOrEmpty(_streamName))
            {
                throw new Exception("Please specify a stream name to get the stream response.");
            }

            var request = new DescribeStreamRequest() { StreamName = streamName ?? _streamName };

            return await _client.DescribeStreamAsync(request);
        }

        public IList<Shard> GetActiveShards()
        {
            return GetShards().Where(m => m.SequenceNumberRange.EndingSequenceNumber == null).ToList();
        }

        public IList<Shard> GetDisabledShards()
        {
            return GetShards().Where(m => m.SequenceNumberRange.EndingSequenceNumber != null).ToList();
        }

        public IList<Shard> GetShards()
        {
            var stream = GetStreamResponse();

            return stream.StreamDescription.Shards;
        }

        public async Task<IList<Shard>> GetDisabledShardsAsync()
        {
            var shards = await GetShardsAsync();

            return shards.Where(m => m.SequenceNumberRange.EndingSequenceNumber != null).ToList();
        }

        public async Task<IList<Shard>> GetActiveShardsAsync()
        {
            var shards = await GetShardsAsync();

            return shards.Where(m => m.SequenceNumberRange.EndingSequenceNumber == null).ToList();
        }

        public async Task<IList<Shard>> GetShardsAsync()
        {
            var stream = await GetStreamResponseAsync();

            return stream.StreamDescription.Shards;
        }

        public IUtilities SetDynamoReadCapacityUnits(int readCapacityUnits)
        {
            _readCapacityUnits = readCapacityUnits;

            return this;
        }

        public IUtilities SetDynamoWriteCapacityUnits(int writeCapacityUnits)
        {
            _writeCapacityUnits = writeCapacityUnits;

            return this;
        }

        public async Task<ListStreamsResponse> ListStreamsAsync(string exclusiveStreamStartName = null)
        {
            var listStreamsRequest = new ListStreamsRequest()
            {
                ExclusiveStartStreamName = exclusiveStreamStartName ?? default(string)
            };

            var response = await _client.ListStreamsAsync(listStreamsRequest);

            return response;
        }

        public ListStreamsResponse ListStreams(string exclusiveStreamStartName = null)
        {
            var listStreamsRequest = new ListStreamsRequest()
            {
                ExclusiveStartStreamName = exclusiveStreamStartName ?? default(string)
            };

            var response = AsyncHelper.RunSync(() => _client.ListStreamsAsync(listStreamsRequest));

            return response;
        }

        public IUtilities SetWorkerId(string workerId)
        {
            _workerId = workerId;

            return this;
        }

        public IUtilities SetStreamName(string streamName)
        {
            _streamName = streamName;

            return this;
        }

        public AddTagsToStreamResponse AddTagsToStream(Dictionary<string, string> tags, string streamName = null)
        {
            if (string.IsNullOrEmpty(streamName) && string.IsNullOrEmpty(_streamName))
            {
                throw new Exception("Please specify a stream name to add tags");
            }

            if (tags == null || tags.Count == 0 || tags.Count > 10)
            {
                throw new Exception("Please create a tag dictionary that's between 0 and 10 in size");
            }

            var addTagsRequest = new AddTagsToStreamRequest()
            {
                StreamName = streamName ?? _streamName,
                Tags = tags
            };

            var response = AsyncHelper.RunSync(() => _client.AddTagsToStreamAsync(addTagsRequest));

            return response;
        }

        public async Task<AddTagsToStreamResponse> AddTagsToStreamAsync(Dictionary<string, string> tags, string streamName = null)
        {
            if (string.IsNullOrEmpty(streamName) && string.IsNullOrEmpty(_streamName))
            {
                throw new Exception("Please specify a stream name to add tags");
            }

            if (tags == null || tags.Count == 0 || tags.Count > 10)
            {
                throw new Exception("Please create a tag dictionary that's between 0 and 10 in size");
            }

            var addTagsRequest = new AddTagsToStreamRequest()
            {
                StreamName = streamName ?? _streamName,
                Tags = tags
            };

            var response = await _client.AddTagsToStreamAsync(addTagsRequest, CancellationToken.None);

            return response;
        }

        public ListTagsForStreamResponse ListTags(string exclusiveStartTagKey = null, string streamName = null)
        {
            if (string.IsNullOrEmpty(streamName) && string.IsNullOrEmpty(_streamName))
            {
                throw new Exception("Please specify a stream name to list tags");
            }

            var listTagsRequest = new ListTagsForStreamRequest()
            {
                StreamName = streamName ?? _streamName,
                ExclusiveStartTagKey = exclusiveStartTagKey,
            };

            var response = AsyncHelper.RunSync(() => _client.ListTagsForStreamAsync(listTagsRequest));

            return response;
        }

        public async Task<ListTagsForStreamResponse> ListTagsAsync(string exclusiveStartTagKey = null, string streamName = null)
        {
            if (string.IsNullOrEmpty(streamName) && string.IsNullOrEmpty(_streamName))
            {
                throw new Exception("Please specify a stream name to list tags");
            }

            var listTagsRequest = new ListTagsForStreamRequest()
            {
                StreamName = streamName ?? _streamName,
                ExclusiveStartTagKey = exclusiveStartTagKey,
            };

            var response = await _client.ListTagsForStreamAsync(listTagsRequest);

            return response;
        }

        public RemoveTagsFromStreamResponse RemoveTagsFromStream(List<string> tagKeys, string streamName = null)
        {
            if (string.IsNullOrEmpty(streamName) && string.IsNullOrEmpty(_streamName))
            {
                throw new Exception("Please specify a stream name to remove tags");
            }

            if (tagKeys == null || tagKeys.Count == 0)
            {
                throw new Exception("Please specify one or more tag keys to remove");
            }

            var removeTagsRequest = new RemoveTagsFromStreamRequest()
            {
                StreamName = streamName ?? _streamName,
                TagKeys = tagKeys
            };
            
            var response = AsyncHelper.RunSync(() => _client.RemoveTagsFromStreamAsync(removeTagsRequest));

            return response;
        }

        public async Task<RemoveTagsFromStreamResponse> RemoveTagsFromStreamAsync(List<string> tagKeys, string streamName = null)
        {
            if (string.IsNullOrEmpty(streamName) && string.IsNullOrEmpty(_streamName))
            {
                throw new Exception("Please specify a stream name to remove tags");
            }

            if (tagKeys == null || tagKeys.Count == 0)
            {
                throw new Exception("Please specify one or more tag keys to remove");
            }

            var removeTagsRequest = new RemoveTagsFromStreamRequest()
            {
                StreamName = streamName ?? _streamName,
                TagKeys = tagKeys
            };

            var response = await _client.RemoveTagsFromStreamAsync(removeTagsRequest);

            return response;
        }
    }
}
