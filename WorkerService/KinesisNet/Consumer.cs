using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;
using Serilog;
using WorkerService.KinesisNet.Extensions;
using WorkerService.KinesisNet.Interface;
using WorkerService.KinesisNet.Model;

namespace WorkerService.KinesisNet
{
    internal class Consumer : IConsumer
    {
        private readonly IAmazonKinesis _client;
        private readonly IUtilities _utilities;
        private readonly IDynamoDB _dynamoDb;

        private CancellationTokenSource _cancellationTokenSource;

        private bool _saveProgressToDynamo;
        private int _currentRecordsProcessing;

        private volatile bool _isRunning;

        public Consumer(IAmazonKinesis client, IUtilities utilities, IDynamoDB dynamoDb)
        {
            _client = client;
            _utilities = utilities;
            _dynamoDb = dynamoDb;

            _isRunning = false;
            _cancellationTokenSource = null;

            _saveProgressToDynamo = true;
            _currentRecordsProcessing = 0;
        }

        public IConsumer EnableSaveToDynamo(bool saveProgress = true)
        {
            _saveProgressToDynamo = saveProgress;

            return this;
        }

        public async Task<Result> Start(IRecordProcessor recordProcessor, string streamName = null)
        {
            if (_isRunning)
            {
                return Result.Create(false, "KinesisNet has already started. Please stop it first.");
            }

            if (!string.IsNullOrEmpty(streamName))
            {
                _utilities.SetStreamName(streamName);
            }

            if (string.IsNullOrEmpty(_utilities.StreamName))
            {
                return Result.Create(false, "Please set a stream name.");
            }

            await _dynamoDb.Init();

            _isRunning = true;
            _currentRecordsProcessing = 0;
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => ProcessShardsAsync(recordProcessor));

            return Result.Create(true, "Processing System");
        }

        public void Stop()
        {
            Log.Information("Stopping Consumer. Worker: {0} & Stream: {1}", _utilities.WorkerId, _utilities.StreamName);

            if (_isRunning)
            {
                _cancellationTokenSource.Cancel();

                _isRunning = false;
            }
        }

        private async Task ProcessShardsAsync(IRecordProcessor processor)
        {
            try
            {
                var processShardsTask = new ConcurrentDictionary<Task<RecordResponse>, KShard>();

                await WatchForShardUpdates(processShardsTask);

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (processShardsTask.Count > 0)
                    {
                        var task = await Task.WhenAny(processShardsTask.Select(m => m.Key)).WithCancellation(_cancellationTokenSource.Token);

                        KShard shard;
                        if (task != null && task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                        {
                            Interlocked.Add(ref _currentRecordsProcessing, 1);

                            if (processShardsTask.TryRemove(task, out shard))
                            {
                                var record = await task;

                                if (record != null && record.GetRecordsResponse.Records.Any())
                                {
                                    processor.Process(shard.ShardId, record.GetRecordsResponse.Records.LastOrDefault().SequenceNumber, shard.LastUpdateUtc, record.GetRecordsResponse.Records, SaveCheckpoint);

                                    if (record.GetRecordsResponse.NextShardIterator != null)
                                    {
                                        shard.UpdateShardInformation(record.GetRecordsResponse);

                                        var getRecordsTask = GetRecordResponse(shard, record.CancellationToken);

                                        processShardsTask.TryAdd(getRecordsTask, shard);
                                    }
                                }
                            }

                            Interlocked.Decrement(ref _currentRecordsProcessing);
                        }
                        else //the task was cancelled or faulted or there is some error
                        {
                            if (task != null)
                            {
                                processShardsTask.TryRemove(task, out shard);
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(2000); //sleep and hope that we get some cool shards to work with
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Log.Debug(e, "ProcessShardsAsync was cancelled");
            }
        }


        private Task WatchForShardUpdates(ConcurrentDictionary<Task<RecordResponse>, KShard> processShardsTask)
        {
            Task.Run(async () =>
            {
                var getRecordCancellationToken = new CancellationTokenSource();

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var newActiveShards = await _utilities.GetActiveShardsAsync();

                    if (newActiveShards.Count != processShardsTask.Count && Interlocked.CompareExchange(ref _currentRecordsProcessing, 0, 0) == 0)
                    {
                        if (processShardsTask.Count > 0) //this is a bit ugly but it'll do - seems robust enough for now
                        {
                            getRecordCancellationToken.Cancel(); //cancel existing record tasks

                            getRecordCancellationToken = new CancellationTokenSource(); //create a new token for any future cancellations
                        }

                        var shards = await _dynamoDb.GetShards(newActiveShards.Select(m => m.ShardId).ToList());

                        //Add new tasks to be processed
                        foreach (var shard in shards)
                        {
                            await GenerateShardIteratorRequest(shard);

                            processShardsTask.TryAdd(GetRecordResponse(shard, getRecordCancellationToken.Token), shard);
                        }

                        Log.Information("Shards Found: {0}", shards.Count);
                    }

                    await Task.Delay(TimeSpan.FromMinutes(2));
                }
            },  _cancellationTokenSource.Token);

            return Task.FromResult(0);
        }

        private async Task<RecordResponse> GetRecordResponse(KShard shard, CancellationToken ctx)
        {
            var request = new GetRecordsRequest { ShardIterator = shard.ShardIterator };

            while (!ctx.IsCancellationRequested)
            {
                var record = await _client.GetRecordsAsync(request, ctx);

                if (record.Records.Count > 0)
                {
                    return RecordResponse.Create(record, ctx);
                }

                request.ShardIterator = record.NextShardIterator;

                await Task.Delay(1000, ctx);
            }

            return RecordResponse.Empty;
        }

        private async Task GenerateShardIteratorRequest(KShard shard)
        {
            var shardIteratorRequest = new GetShardIteratorRequest()
            {
                ShardId = shard.ShardId,
                ShardIteratorType = shard.ShardIteratorType,
                StartingSequenceNumber = shard.SequenceNumber,
                StreamName = _utilities.StreamName
            };

            var response = await _client.GetShardIteratorAsync(shardIteratorRequest, _cancellationTokenSource.Token);

            shard.SetNextShardIterator(response.ShardIterator);
        }

        private void SaveCheckpoint(string shardId, string sequenceNumber, DateTime lastUpdateUtc)
        {
            if (_saveProgressToDynamo)
            {
                _dynamoDb.SaveToDatabase(shardId, sequenceNumber, lastUpdateUtc).Wait();
            }
        }
    }
}
