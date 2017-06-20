namespace KinesisNet.Interface
{
    public interface IKManager
    {
        IConsumer Consumer { get; }
        IProducer Producer { get; }
        IUtilities Utilities { get; }
    }
}