using System.Threading;
using Amazon.Kinesis.Model;

namespace KinesisNet.Model
{
    internal class RecordResponse
    {
        public GetRecordsResponse GetRecordsResponse { get; private set; }
        public CancellationToken CancellationToken { get; private set; }

        public static RecordResponse Create(GetRecordsResponse getRecordsResponse, CancellationToken cancellationToken)
        {
            return new RecordResponse()
            {
                GetRecordsResponse = getRecordsResponse,
                CancellationToken = cancellationToken
            };
        }

        public static RecordResponse Empty 
        {
            get
            {
                return new RecordResponse();
            }
        }
    }
}
