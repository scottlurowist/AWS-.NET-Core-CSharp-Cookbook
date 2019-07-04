using System;
using System.Threading;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace S3Doodles
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello To S3!");

            AWSCredentials credsBase = new BasicAWSCredentials("AKIAUO6ZESUM4FSFKYGQ",
                "L0vOC02MkwGXLIlRqzD8YdSg6sg16dDEyMgujsgx");
            
            IAmazonS3 client = new AmazonS3Client(credsBase, RegionEndpoint.APNortheast1);
            CancellationToken token = new CancellationToken();

            var foo = client.ListBucketsAsync(token);

            

            foreach (var bucket in foo.Result.Buckets)
            {
                Console.WriteLine(bucket.BucketName);

                CancellationToken newToken = new CancellationToken();

                var bar = client.ListObjectsAsync(bucket.BucketName, newToken);

                var foobar = bar.Result.S3Objects;

                //foreach (var someObj in bar.Result.S3Objects)
                //{
                //    Console.WriteLine($"\t\t\t{someObj.Key}");
                //}
            }

            client.Dispose();

            Console.WriteLine();
        }
    }
}
