using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using static AzureStorageDemo.Utils;

namespace AzureStorageDemo
{
    public class Person
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] Avatar { get; set; }
    }

    public static class MongoCosmosDb
    {
        private const string ConnString =
            "mongodb://6f0d4f20-0ee0-4-231-b9ee:TfO9s3DlvlLZdtmfK7824aMlOrmPh9pkcit14DxBVFGoC9zZIkrCEdp4C64yl4C22vicmzioBk3qUP7UYSZHuw==@6f0d4f20-0ee0-4-231-b9ee.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

        public static async Task<TimeSpan> InsertPersons(int numRows)
        {
            const int batchSize = 10;
            var client = new MongoClient(ConnString);
            var db = client.GetDatabase("azurectmongo");
            var personCollection = db.GetCollection<Person>("Persons");
            var batchedPersonsToInsert = GenerateRandomPersons(numRows)
                .Select((p, idx) => (Person: p, BatchId: idx % batchSize))
                .GroupBy(x => x.BatchId);

            return await Time(async () =>
            {
                foreach (var batch in batchedPersonsToInsert)
                {
                    await personCollection.InsertManyAsync(batch.Select(b => b.Person));
                }
            });
        }

        private static IEnumerable<Person> GenerateRandomPersons(int numRows)
        {
            using (var surnamesIterator = InfiniteSurnames().GetEnumerator())
            {
                var random = new Random();
                var avatar = new byte[64 * 1024];

                return Enumerable.Range(0, numRows)
                    .Select(n =>
                    {
                        surnamesIterator.MoveNext();
                        random.NextBytes(avatar);
                        var personToInsert = new Person
                        {
                            Id = Guid.NewGuid(),
                            FirstName = RandomString(20, random),
                            LastName = surnamesIterator.Current,
                            Avatar = avatar
                        };
                        return personToInsert;
                    })
                    .ToList();
            }
        }
    }
}
