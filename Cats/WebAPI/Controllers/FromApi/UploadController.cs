using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.Json;
using Application.Core;
using Application.Infrastructure.FilesManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UploadController : ApiRoutedController
    {
        private readonly LazyDependency<IFilesManagementService> _filesManagementService =
            new LazyDependency<IFilesManagementService>();

        private readonly string _storageRoot = ConfigurationManager.AppSettings["FileUploadPath"];

        private readonly string _storageRootTemp = ConfigurationManager.AppSettings["FileUploadPathTemp"];

        public IFilesManagementService FilesManagementService => _filesManagementService.Value;

        #region UploadController Members

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpDelete("DeleteFiles")]
        public IActionResult DeleteFiles(string filename)
        {
            var file = _storageRootTemp + "/" + filename;
            if (System.IO.File.Exists(file)) System.IO.File.Delete(file);

            return Ok();
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpGet("DownloadFile")]
        public IActionResult DownloadFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName)) return Ok(DownloadFileContent(fileName));

            return BadRequest();
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpGet("GetUploadedFiles")]
        public IActionResult GetUploadedFiles(string values, string deleteValues)
        {
            return !string.IsNullOrEmpty(values) ? _GetUploadedFiles(values, deleteValues) : BadRequest();
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpPost("UploadFiles")]
        public IActionResult UploadFiles()
        {
            return Ok(_UploadFile(HttpContext));
        }

        #endregion UploadController Members

        #region Private Members

        private IActionResult DownloadFileContent(string fileName)
        {
            var filePath = _storageRoot + fileName;

            if (!System.IO.File.Exists(filePath)) return null;

            //var response = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read))
            //};
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            //if (/*HttpContext.Request.Browser.Browser == "IE"*/false)
            //{
            //    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            //    {
            //        FileName = HttpUtility.UrlPathEncode(
            //            FilesManagementService.GetFileDisplayName(Path.GetFileName(filePath)))
            //    };
            //}
            //else
            //{
            //    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            //    {
            //        FileName = FilesManagementService.GetFileDisplayName(Path.GetFileName(filePath))
            //    };
            //}

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return File(fileStream, "application/octet-stream");
        }

        private IActionResult _GetUploadedFiles(string filesPath, string deleteValues)
        {
            var values = JsonSerializer.Deserialize<List<string>>(filesPath);
            var files = values
                .Select(value => value.Split(new[] {'/'}))
                .Select(split => new AttachedFile(
                    split[0],
                    split[3],
                    new FileInfo(_storageRoot + split[2] + "//" + split[3]),
                    Convert.ToInt32(split[1]),
                    deleteValues))
                .ToList();

            return Ok(files);
        }

        private string _GetFileType(IFormFile fileType)
        {
            var split = fileType.ContentType.Split(new[] {'/'});
            return split[0] switch
            {
                "image" => "Image",
                "video" => "Video",
                "audio" => "Audio",
                _ => "Document"
            };
        }

        private IEnumerable<AttachedFile> _UploadFile(HttpContext context)
        {
            var statuses = new List<AttachedFile>();

            foreach (var file in context.Request.Form.Files)
            {
                var guidFileName = GetGuidFileName() + Path.GetExtension(file.FileName.ToLower());
                var fullPath = _storageRootTemp + guidFileName;

                //todo # check this implementation
                using var readStream = file.OpenReadStream();
                using var fileStream = System.IO.File.Create(fullPath);
                readStream.CopyTo(fileStream);

                statuses.Add(new AttachedFile(Path.GetFileName(file.FileName), guidFileName, new FileInfo(fullPath),
                    (int) file.Length, fullPath, "DELETE"));
            }

            return statuses;
        }

        private string GetGuidFileName()
        {
            return $"N{Guid.NewGuid().ToString("N").ToUpper()}";
        }

        #endregion Private Members
    }
}