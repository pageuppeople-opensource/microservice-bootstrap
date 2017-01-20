using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Serilog;
using WorkerService.KinesisNet.Interface;
using WorkerService.KinesisNet.Model;

namespace WorkerService.KinesisNet.Persistance
{
    internal class DynamoDB : IDynamoDB
    {
        private readonly IAmazonDynamoDB _client;
        private const string TableName = "kinesisnet_checkpoint";
        private const string KeyIdPattern = "{0}+{1}+{2}";

        private bool _tableExists;

        private readonly IUtilities _utilities;

        public DynamoDB(IAmazonDynamoDB client, IUtilities utilities)
            : this(utilities)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            _client = client;
        }

        public DynamoDB(string awsKey, string awsSecret, RegionEndpoint regionEndpoint, IUtilities utilities)
            : this(utilities)
        {
            _client = new AmazonDynamoDBClient(awsKey, awsSecret, new AmazonDynamoDBConfig { RegionEndpoint = regionEndpoint });

        }

        public DynamoDB(string awsKey, string awsSecret, string awsSessionToken, RegionEndpoint regionEndpoint, IUtilities utilities)
            : this(utilities)
        {
            _client = new AmazonDynamoDBClient(awsKey, awsSecret, awsSessionToken, new AmazonDynamoDBConfig { RegionEndpoint = regionEndpoint });
        }

        public DynamoDB(IUtilities utilities)
        {
            _tableExists = false;

            _utilities = utilities;
        }

        public async Task Init()
        {
            await CreateTableIfNotExists();
        }

        public async Task SaveToDatabase(string shardId, string sequenceNumber, DateTime lastUpdateUtc)
        {
            if (!_tableExists)
            {
                Log.Error("The DynamoDB checkpoint table doesn't exist.");

                return;
            }

            var putItemRequest = new PutItemRequest {TableName = TableName};

            putItemRequest.Item.Add("Id", new AttributeValue(string.Format(KeyIdPattern, shardId, _utilities.WorkerId, _utilities.StreamName)));
            putItemRequest.Item.Add("ShardId", new AttributeValue(shardId));

            if (!string.IsNullOrEmpty(sequenceNumber))
            {
                putItemRequest.Item.Add("SequenceNumber", new AttributeValue(sequenceNumber));
            }

            putItemRequest.Item.Add("LastUpdate", new AttributeValue(lastUpdateUtc.ToString()));

            try
            {
                await _client.PutItemAsync(putItemRequest);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
            }
        }

        private Task<QueryResponse> CreateQueryRequest(string shardId)
        {
            var key = new AttributeValue(string.Format(KeyIdPattern, shardId, _utilities.WorkerId, _utilities.StreamName));

            var request = new QueryRequest(TableName)
            {
                KeyConditions = new Dictionary<string, Condition>()
                {
                    {
                        "Id",
                        new Condition()
                        {
                            ComparisonOperator = "EQ",
                            AttributeValueList = new List<AttributeValue>(){{key}}
                        }
                    }
                }
            };

            return _client.QueryAsync(request);
        }

        public async Task<List<KShard>> GetShards(IList<string> shardIds)
        {
            var list = new List<KShard>();
            
            if (!shardIds.Any())
            {
                return list;
            }

            var queries = shardIds.Select(CreateQueryRequest).ToList();

            try
            {
                while (queries.Any())
                {
                    var response = await Task.WhenAny(queries);

                    queries.Remove(response);

                    var result = await response;

                    if (result.Items.Count > 0)
                    {
                        foreach (var item in result.Items)
                        {
                            AttributeValue shardId, sequenceNumber;

                            if (item.TryGetValue("ShardId", out shardId) && item.TryGetValue("SequenceNumber", out sequenceNumber))
                            {
                                list.Add(new KShard(shardId.S, _utilities.StreamName, Amazon.Kinesis.ShardIteratorType.AFTER_SEQUENCE_NUMBER, sequenceNumber.S));

                                shardIds.Remove(shardId.S);
                            }
                        }
                    }
                }

                list.AddRange(shardIds.Select(shardId => new KShard(shardId, _utilities.StreamName))); //add remaining shards that were not saved before

                return list;
            }
            catch (AmazonDynamoDBException e)
            {
                Debug.WriteLine(e);

                throw e;
            }
        }

        private async Task CreateTableIfNotExists()
        {
            var listTablesRequest = new ListTablesRequest { ExclusiveStartTableName = TableName.Substring(0, 6) };

            try
            {

                Log.Information("Checking if kinesis_checkpoint table exists");

                var listTables = await _client.ListTablesAsync(listTablesRequest);

                Log.Debug("{@listTables}", listTables);

                if (listTables.TableNames.Contains(TableName))
                {

                    Log.Debug("Table exists");
                    _tableExists = true;
                }
                else
                {
                    var request = new CreateTableRequest()
                    {
                        TableName = TableName,
                        AttributeDefinitions = new List<AttributeDefinition>()
                    {
                        new AttributeDefinition()
                        {
                            AttributeName = "Id",
                            AttributeType = "S",
                        }
                    },
                        KeySchema = new List<KeySchemaElement>()
                    {
                        new KeySchemaElement()
                        {
                            AttributeName = "Id",
                            KeyType = "HASH"
                        }
                    },
                        ProvisionedThroughput = new ProvisionedThroughput()
                        {
                            ReadCapacityUnits = _utilities.DynamoReadCapacityUnits,
                            WriteCapacityUnits = _utilities.DynamoWriteCapacityUnits
                        },
                    };


                    Log.Information("Creating dyanmo table for kinesis checkpoints...");

                    var response = await _client.CreateTableAsync(request);

                    if (response.HttpStatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Could not create dynamodb table to store checkpoints");
                    }

                    //wait while table is created
                    var tableStatus = response.TableDescription.TableStatus;

                    while (tableStatus != TableStatus.ACTIVE)
                    {
                        var checkTableStatus = await _client.DescribeTableAsync(new DescribeTableRequest(TableName));

                        tableStatus = checkTableStatus.Table.TableStatus;

                        Log.Information("Waiting for dynamo table creation...");

                        Thread.Sleep(500);
                    }

                    Log.Information("The DynamoDB checkpoint table created.");

                    _tableExists = true;
                }
            }
            catch (Exception e)
            {
                Log.Debug(e, "Could not connect to DynamoDB");

                throw;
            }
        }
    }
}
