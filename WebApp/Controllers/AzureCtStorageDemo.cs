using System.Threading.Tasks;
using AzureStorageDemo;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AzureCtStorageDemoController: Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BulkInsert(int numRows)
        {
            var timeTaken = await SqlBulkInsert.BulkCopy(numRows);
            return View("ShowResult", timeTaken);
        }

        [HttpPost]
        public async Task<IActionResult> BlobStorage(int blobKb)
        {
            var timeTaken = await BlockBlobs.TestBlobStorage(blobKb);
            return View("ShowResult", timeTaken);
        }

        [HttpPost]
        public async Task<IActionResult> TableStorage(int numRows)
        {
            var timeTaken = await AzureTableStorage.TableStorageBatched(numRows);
            return View("ShowResult", timeTaken);
        }

        [HttpPost]
        public async Task<IActionResult> MongoCosmosDbInsert(int numRows)
        {
            var timeTaken = await MongoCosmosDb.InsertPersons(numRows);
            return View("ShowResult", timeTaken);
        }

        [HttpPost]
        public async Task<IActionResult> QueueStorageStore(int num32kStrings)
        {
            var timeTaken = await QueueStorage.StoreMessages(num32kStrings);
            return View("ShowResult", timeTaken);
        }
        
    }
}
