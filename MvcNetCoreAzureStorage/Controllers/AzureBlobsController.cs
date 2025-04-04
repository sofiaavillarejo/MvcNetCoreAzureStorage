﻿using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using MvcNetCoreAzureStorage.Models;
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
        public async Task<IActionResult> ListBlobs(string containername)
        {
            List<BlobModel> models = await this.service.GetBlobsAsync(containername);
            return View(models);
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
            return RedirectToAction("ListBlobs", new { containername = containername });
        }

        public async Task<IActionResult> DeleteBlob(string containername, string blobname)
        {
            await this.service.DeleteBlobAsync(containername, blobname);
            return RedirectToAction("ListBlobs", new { containername = containername });
        }

        public async Task<IActionResult> GetImagen(string containerName, string nombreBlob)
        {
            BlobContainerClient containerClient = this.service.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(nombreBlob);

            var response = await blobClient.DownloadAsync();
            Stream stream = response.Value.Content;

            return File(stream, "image/jpeg");
        }

    }
}
