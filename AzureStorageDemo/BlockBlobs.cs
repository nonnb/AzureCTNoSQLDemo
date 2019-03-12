using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using static AzureStorageDemo.Utils;

namespace AzureStorageDemo
{
    [TestClass]
    public class BlockBlobs
    {
        private const string ConnString =
            "{yourconnectionstringhere}";

        public static async Task<TimeSpan> TestBlobStorage(int blobKilobytes)
        {
            var container = await GetContainerReference("bigblobs");
            var blobKey = Guid.NewGuid().ToString("N");
            var blockBlob = container.GetBlockBlobReference(blobKey);
            var random = new Random();
            var blobBytes = new byte[blobKilobytes * 1024];
            random.NextBytes(blobBytes);

            return await Time(
                async () =>
            {
                await blockBlob.UploadFromByteArrayAsync(blobBytes, 0, blobBytes.Length);
            });
        }

        private static async Task<CloudBlobContainer> GetContainerReference(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(ConnString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var blobContainer = blobClient.GetContainerReference(containerName);

            // TODO : In an app, just cache the Container Reference in a ConcurrentDictionary once created / found
            await blobContainer.CreateIfNotExistsAsync();
            return blobContainer;
        }
    }
}
