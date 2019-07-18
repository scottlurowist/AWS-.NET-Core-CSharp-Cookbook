using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBCookbook
{
    class Program
    {
        private const string MyTableName = "SomeFilmsThatILike";
        

        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome To DynamoDB!\n\n");

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            using (IAmazonDynamoDB client = new AmazonDynamoDBClient(RegionEndpoint.USEast1))
            {
                await CreateTable(client, token);
                await DeleteTable(client, token);
            }
        }

        private static async Task CreateTable(IAmazonDynamoDB client, CancellationToken token)
        {
            CreateTableRequest request = new CreateTableRequest()
            {
                TableName = MyTableName,
                AttributeDefinitions =
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "N"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "Movie",
                        AttributeType = "S"
                    }
                },
                KeySchema =
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH" // Partition Key
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "Movie",
                        KeyType = "Range" // Sort Key
                    }
                },

                BillingMode = BillingMode.PAY_PER_REQUEST
            };

            await client.CreateTableAsync(request, token);

            DescribeTableResponse response;

            do
            {
                Thread.Sleep(1000);
                Console.WriteLine($"Creating table {MyTableName}...");

                response = await client.DescribeTableAsync(MyTableName, token);

            } while (response.Table.TableStatus == TableStatus.CREATING);

            if (response.Table.TableStatus == TableStatus.ACTIVE)
            {
                Console.WriteLine($"\nTable {MyTableName} was successfully created.");
            }

            Console.WriteLine("\nPress any key to continue...\n");
            Console.ReadKey();
        }


        private static async Task DeleteTable(IAmazonDynamoDB client, CancellationToken token)
        {
            DeleteTableRequest request = new DeleteTableRequest(MyTableName);

            await client.DeleteTableAsync(request, token);

            DescribeTableResponse response;

            string expectedExceptionMessage = 
                $"Requested resource not found: Table: {MyTableName} not found";

            try
            {
                do
                {
                    Thread.Sleep(2000);
                    Console.WriteLine($"Deleting table {MyTableName}...");

                    response = await client.DescribeTableAsync(MyTableName, token);

                } while (response.Table.TableStatus == TableStatus.DELETING);
            }
            catch (ResourceNotFoundException e) when (CheckDeleteTableException(e, expectedExceptionMessage))
            {
                Console.WriteLine($"\nTable {MyTableName} was successfully deleted.");
            }

            Console.WriteLine("\nPress any key to continue...\n");
            Console.ReadKey();
        }


        private static bool CheckDeleteTableException(Exception excp, string expectedExceptionMessage)
        {
            return excp.Message.Contains(expectedExceptionMessage) &&
                   excp.Source == "AWSSDK.Core";
        }
    }
}
