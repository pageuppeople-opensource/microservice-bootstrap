using System.Threading.Tasks;
using KinesisNet.Interface;
using KinesisNet.Model;

namespace KinesisNet.Interface
{
    public interface IConsumer
    {
        IConsumer EnableSaveToDynamo(bool saveProgress = true);

        Task<Result> Start(IRecordProcessor recordProcessor, string streamName = null);
        void Stop();
    }
}
