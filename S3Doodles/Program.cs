using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3Doodles
{
    class Program
    {
        private const string BucketName = "scottlurowist";
        private const string ImageName = "Tiger-Sambar-09.03.2013-10.40PM.JPG";


        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome To S3!");

            using (IAmazonS3 client = new AmazonS3Client(RegionEndpoint.USEast1))
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                await ListAllBucketsAsync(client, token);
                await ListContentsOfAwsCookbookAsync(client, token);
                await UploadImageAsync(client, token);
                await DownloadImageToDesktopAsync(client, token);
            }
        }

        private static async Task ListAllBucketsAsync(IAmazonS3 client, CancellationToken token)
        {
            ListBucketsResponse response = await client.ListBucketsAsync(token);

            foreach (var bucket in response.Buckets)
            {
                Console.WriteLine(bucket.BucketName);
            }
        }

        private static async Task ListContentsOfAwsCookbookAsync(IAmazonS3 client, CancellationToken token)
        {
            ListObjectsResponse response =
                await client.ListObjectsAsync(BucketName, "AWS Cookbook", token);

            Console.WriteLine($"\nListing the contents of prefix 'AWS Cookbook'\n");

            foreach (var resultS3Object in response.S3Objects)
            {
                Console.WriteLine($"   {resultS3Object.Key}");
            }
        }

        private static async Task UploadImageAsync(IAmazonS3 client, CancellationToken token)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = BucketName,
                FilePath = ImageName,
                Key = ImageName,
                StorageClass = S3StorageClass.OneZoneInfrequentAccess
            };

            PutObjectResponse response = await client.PutObjectAsync(request, token);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"\n\nPhoto {ImageName} was successfully uploaded to bucket 'scottlurowist'");
            }
        }


        private static async Task DownloadImageToDesktopAsync(IAmazonS3 client, CancellationToken token)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string filePath = Path.Combine(desktopPath, ImageName);

            GetObjectResponse getObjResponse =
                await client.GetObjectAsync(BucketName, ImageName, token);

            await getObjResponse.WriteResponseStreamToFileAsync(filePath,
                false, token);

            Console.WriteLine($"Image {ImageName} was successfully downloaded to {desktopPath}.");
        }
    }
}
