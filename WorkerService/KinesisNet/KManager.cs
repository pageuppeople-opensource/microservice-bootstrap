using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Kinesis;
using Serilog;
using WorkerService.KinesisNet.Interface;
using WorkerService.KinesisNet.Persistance;

namespace WorkerService.KinesisNet
{
    public class KManager : IKManager
    {
        private readonly IAmazonKinesis _client;

        private IConsumer _consumer = null;
        private IProducer _producer = null;
        private IUtilities _utilities = null;
        private readonly IDynamoDB _dynamoDb = null;

        public KManager(string awsKey, string awsSecret, string streamName, AmazonKinesisConfig config, string workerId = null) : 
            this(awsKey, awsSecret, config, workerId)
        {
            _utilities.SetStreamName(streamName);
        }

        public KManager(string awsKey, string awsSecret, string streamName, AmazonKinesisConfig config, string awsSessionToken, string workerId) :
            this(awsKey, awsSecret, config, awsSessionToken, workerId)
        {
            _utilities.SetStreamName(streamName);
        }

        public KManager(string awsKey, string awsSecret, string streamName, RegionEndpoint regionEndpoint, string workerId = null) : 
            this(awsKey, awsSecret, streamName, new AmazonKinesisConfig() {RegionEndpoint = regionEndpoint}, workerId)
        {
            _utilities.SetStreamName(streamName);
        }

        public KManager(string awsKey, string awsSecret, string streamName, RegionEndpoint regionEndpoint, string awsSessionToken, string workerId) :
            this(awsKey, awsSecret, streamName, new AmazonKinesisConfig() { RegionEndpoint = regionEndpoint }, awsSessionToken, workerId)
        {
            _utilities.SetStreamName(streamName);
        }

        public KManager(string awsKey, string awsSecret, RegionEndpoint regionEndpoint, string workerId = null) :
            this(awsKey, awsSecret, new AmazonKinesisConfig() { RegionEndpoint = regionEndpoint }, workerId)
        {
        }

        public KManager(string awsKey, string awsSecret, RegionEndpoint regionEndpoint, string awsSessionToken, string workerId) :
            this(awsKey, awsSecret, new AmazonKinesisConfig() { RegionEndpoint = regionEndpoint }, awsSessionToken, workerId)
        {
        }

        public KManager(string awsKey, string awsSecret, AmazonKinesisConfig config, string workerId = null)
        {
            if (workerId == null)
            {
                workerId = System.Environment.MachineName;
            }

            _client = new AmazonKinesisClient(awsKey, awsSecret, config);

            _utilities = new Utilities(_client, workerId);
            _dynamoDb = new DynamoDB(awsKey, awsSecret, config.RegionEndpoint, _utilities);

            _producer = new Producer(_client, _utilities);
            _consumer = new Consumer(_client, _utilities, _dynamoDb);

            Log.Information("Instantiated KManager");
        }

        public KManager(string awsKey, string awsSecret, AmazonKinesisConfig config, string awsSessionToken, string workerId)
        {
            if (workerId == null)
            {
                workerId = System.Environment.MachineName;
            }

            _client = new AmazonKinesisClient(awsKey, awsSecret, awsSessionToken, config);

            _utilities = new Utilities(_client, workerId);
            _dynamoDb = new DynamoDB(awsKey, awsSecret, awsSessionToken, config.RegionEndpoint, _utilities);

            _producer = new Producer(_client, _utilities);
            _consumer = new Consumer(_client, _utilities, _dynamoDb);

            Log.Information("Instantiated KManager");
        }

        public KManager(IAmazonDynamoDB dynamoClient, IAmazonKinesis kinesisClient, string streamName, string workerId)
        {
            Log.Debug("Entering KManager ctor");
            if (workerId == null)
            {
                workerId = System.Environment.MachineName;
            }

            _client = kinesisClient;
            Log.Debug("Creating Utitlities");
            _utilities = new Utilities(_client, workerId);
            Log.Debug("Creating Dynamo presistance");
            _dynamoDb = new DynamoDB(dynamoClient,  _utilities);

            Log.Debug("Creating Producer")
            _producer = new Producer(_client, _utilities);
            Log.Debug("Creating Consumer");
            _consumer = new Consumer(_client, _utilities, _dynamoDb);


            _utilities.SetStreamName(streamName);

            Log.Information("Instantiated KManager");
        }

        private void Init(string workerId = null)
        {
            if (workerId == null)
            {
                workerId = System.Environment.MachineName;
            }

            _utilities = new Utilities(_client, workerId);

            _producer = new Producer(_client, _utilities);
            _consumer = new Consumer(_client, _utilities, _dynamoDb);

            Log.Information("Instantiated KManager");
        }

        public IConsumer Consumer
        {
            get { return _consumer; }
        }

        public IProducer Producer
        {
            get { return _producer; }
        }

        public IUtilities Utilities
        {
            get { return _utilities; }
        }
    }
}
