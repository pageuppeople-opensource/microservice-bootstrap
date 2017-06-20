using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Kinesis.Model;
using Serilog;

namespace KinesisNet.Interface
{
    public interface IUtilities
    {
        string StreamName { get; }
        string WorkerId { get; }
        SplitShardResponse SplitShard(Shard shard);
        MergeShardsResponse MergeShards(string leftShard, string rightShard);

        DescribeStreamResponse GetStreamResponse(string streamName = null);
        Task<DescribeStreamResponse> GetStreamResponseAsync(string streamName = null);

        IList<Shard> GetActiveShards();
        IList<Shard> GetDisabledShards();
        IList<Shard> GetShards();
        Task<IList<Shard>> GetDisabledShardsAsync();
        Task<IList<Shard>> GetActiveShardsAsync();
        Task<IList<Shard>> GetShardsAsync();

        int DynamoReadCapacityUnits { get; }
        int DynamoWriteCapacityUnits { get; }

        IUtilities SetDynamoReadCapacityUnits(int readCapacityUnits);
        IUtilities SetDynamoWriteCapacityUnits(int writeCapacityUnits);
        IUtilities SetStreamName(string streamName);
        IUtilities SetWorkerId(string workerId);
        IUtilities SetLogConfiguration(LoggerConfiguration configuration);

        ILogger Log { get; }
        Task<ListStreamsResponse> ListStreamsAsync(string exclusiveStreamStartName = null);
        ListStreamsResponse ListStreams(string exclusiveStreamStartName = null);
        AddTagsToStreamResponse AddTagsToStream(Dictionary<string, string> tags, string streamName = null);
        Task<AddTagsToStreamResponse> AddTagsToStreamAsync(Dictionary<string, string> tags, string streamName = null);
        ListTagsForStreamResponse ListTags(string exclusiveStartTagKey = null, string streamName = null);
        Task<ListTagsForStreamResponse> ListTagsAsync(string exclusiveStartTagKey = null, string streamName = null);
        RemoveTagsFromStreamResponse RemoveTagsFromStream(List<string> tagKeys, string streamName = null);
        Task<RemoveTagsFromStreamResponse> RemoveTagsFromStreamAsync(List<string> tagKeys, string streamName = null);
    }
}
