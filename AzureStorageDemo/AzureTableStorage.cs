using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using static AzureStorageDemo.Utils;

namespace AzureStorageDemo
{
    public class PersonRow : TableEntity
    {
        public PersonRow(string lastName, string firstName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
        }

        public byte[] Avatar { get; set; }
    }

    [TestClass]
    public class AzureTableStorage
    {
        private const string ConnString = "";

        private static readonly IEnumerable<string> Surnames = new[]
        {
            "Smith",
            "Jones",
            "Federer",
            "Foobar-Bazton",
            "LoadTest",
            "Messi",
            "Pear",
            "Reddington",
            "Ronaldo",
            "Smurf",
            "Teaspoon",
            "TestUser",
            "van der Merwe"
        };

        private static IEnumerable<string> InfiniteSurnames()
        {
            while (true)
            {
                foreach (var surname in Surnames)
                {
                    yield return surname;
                }
            }
        }

        [TestMethod]
        public async Task TableStorageBatched()
        {
            var numRows = 1000;
            // Max 100 rows, and max Total payload allowed is 4MB
            // Let's say we're 100kB / Person, so ~40
            const int batchSize = 40;
            var table = await GetTableReference();
            var surnamesIterator = InfiniteSurnames().GetEnumerator();
            var random = new Random();
            var avatar = new byte[64 * 1024];
            var batchedPersonsToInsert = Enumerable.Range(0, numRows)
                .Select(n =>
                {
                    surnamesIterator.MoveNext();
                    random.NextBytes(avatar);
                    var personToInsert = new PersonRow(surnamesIterator.Current, RandomString(20, random))
                    {
                        Avatar = avatar
                    };
                    return personToInsert;
                })
                // All rows in the same batch must have same partition key
                // Order by surnames (the partition key)
                .GroupBy(p => p.PartitionKey)
                .SelectMany(p => p.Select((person, idx) => (person, idx)))
                // Deliberate int trunc on the batchsize, i.e. 0..batchSize => 0, then 1 etc
                .GroupBy(x => (x.person.PartitionKey, x.idx / batchSize));

            await Time($"Inserting {numRows} persons",
                async () =>
                {
                    foreach (var personBatch in batchedPersonsToInsert)
                    {
                        var tableStoreBatchOp = new TableBatchOperation();
                        foreach (var person in personBatch)
                        {
                            tableStoreBatchOp.Insert(person.person);
                        }
                        await table.ExecuteBatchAsync(tableStoreBatchOp);
                    }
                });
        }

        private static async Task<CloudTable> GetTableReference()
        {
            var storageAccount = CloudStorageAccount.Parse(ConnString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("foos");
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
