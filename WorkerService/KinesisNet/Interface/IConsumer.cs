using WorkerService.KinesisNet.Model;

namespace WorkerService.KinesisNet.Interface
{
    public interface IConsumer
    {
        IConsumer EnableSaveToDynamo(bool saveProgress = true);

        Result Start(IRecordProcessor recordProcessor, string streamName = null);
        void Stop();
    }
}
