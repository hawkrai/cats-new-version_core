using System;
using System.Configuration;
using System.Linq;
using Application.Core;
using Application.Infrastructure.FilesManagement;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.Files;

namespace WebAPI.Controllers.Services.Files
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FilesServiceController : ApiRoutedController
    {
        private readonly LazyDependency<IFilesManagementService> _filesManagementService =
            new LazyDependency<IFilesManagementService>();

        public IFilesManagementService FilesManagementService => _filesManagementService.Value;

        [HttpGet("GetFiles")]
        public IActionResult GetFiles()
        {
            try
            {
                var attachments = FilesManagementService.GetAttachments(null).ToList();
                var storageRoot = ConfigurationManager.AppSettings["FileUploadPath"];
                var result = new AttachmentResult
                {
                    Files = attachments,
                    ServerPath = storageRoot
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }
    }
}