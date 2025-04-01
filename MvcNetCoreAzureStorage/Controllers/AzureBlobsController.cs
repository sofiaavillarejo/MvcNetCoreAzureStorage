using Microsoft.AspNetCore.Mvc;
using MvcNetCoreAzureStorage.Services;

namespace MvcNetCoreAzureStorage.Controllers
{
    public class AzureBlobsController : Controller
    {
        private ServiceStorageBlobs service;

        public AzureBlobsController(ServiceStorageBlobs service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<string> containers = await this.service.GetContainersAsync();
            return View(containers);
        }

        public IActionResult CreateContainer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainer(string containername)
        {
            await this.service.CreateContainerAsync(containername);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteContainer(string containername)
        {
            await this.service.DeleteContainerAsync(containername);
            return RedirectToAction("Index");
        }

        public IActionResult UploadBlob(string containername)
        {
            ViewData["CONTAINER"] = containername;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBlob(string containername, IFormFile file)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadBlobAsync(containername, blobName, stream);
            }
            return RedirectToAction("VerBlobsContainer", new { containername = containername });
        }
    }
}
