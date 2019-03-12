using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureStorageDemo
{
    public class QueueStorage
    {
        private const string ConnString =
            "DefaultEndpointsProtocol=https;AccountName=ctazurestorage;AccountKey=rsiC2YWwRDF2uRhzJmo38VGYjtaz+sBuOOei8w0aNuj0xTyGhZuicxcCftnvE4rkCOMbYa8cb+RNTMoTbTbacQ==;EndpointSuffix=core.windows.net";

        public static async Task<TimeSpan> StoreMessages(int num32kStrings)
        {
            var storageAccount = CloudStorageAccount.Parse(ConnString);
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();
            var queue = cloudQueueClient.GetQueueReference("myqueue");
            await queue.CreateIfNotExistsAsync();
            
            return await Utils.Time(async () =>
            {
                // Apols, couldn't find a "bulk", so use async parallelization
                var tasks = Enumerable.Range(0, num32kStrings)
                    .Select(i =>
                    {
                        // Unicode string but is Utf8 Encoded, so ~1 byte / char
                        var messageToSend = Utils.RandomString(32 * 1000);
                        return queue.AddMessageAsync(new CloudQueueMessage(messageToSend));
                    });
                await Task.WhenAll(tasks);
            });
        }
    }
}