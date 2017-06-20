using System.Threading.Tasks;
using Amazon.Kinesis.Model;

namespace KinesisNet.Interface
{
    public interface IProducer
    {
        Task<PutRecordResponse> PutRecordAsync(string record, string partitionKey = null);
        Task<PutRecordResponse> PutRecordAsync(byte[] data, string partitionKey = null);

        PutRecordResponse PutRecord(string record, string partitionKey = null);
        PutRecordResponse PutRecord(byte[] data, string partitionKey = null);
    }
}
