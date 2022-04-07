using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcCoreAWSS3.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAWSS3.Controllers
{
    public class AwsFilesController : Controller
    {
        private ServiceAWSS3 service;

        public AwsFilesController(ServiceAWSS3 service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<string> files = await this.service.GetFilesAsync();
            return View(files);
        }

        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadFileAsync(stream, file.FileName);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FileAWS(string fileName)
        {
            Stream stream =
                await this.service.GetFileAsync(fileName);
            return File(stream, "image/png");
        }

        public async Task<IActionResult> Delete(string fileName)
        {
            await this.service.DeleteFileAsync(fileName);
            return RedirectToAction("Index");
        }
    }
}
