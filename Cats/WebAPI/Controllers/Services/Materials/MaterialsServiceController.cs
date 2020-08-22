using System;
using System.Linq;
using Application.Core;
using Application.Infrastructure.MaterialsManagement;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.Materials;

namespace WebAPI.Controllers.Services.Materials
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MaterialsServiceController : ApiRoutedController
    {
        private readonly LazyDependency<IMaterialsManagementService> _materialsManagementService =
            new LazyDependency<IMaterialsManagementService>();

        public IMaterialsManagementService MaterialsManagementService => _materialsManagementService.Value;

        [HttpGet("GetFoldersMaterials")]
        public IActionResult GetFolders(string Pid, string subjectId)
        {
            var pid = int.Parse(Pid);
            var subjectid = int.Parse(subjectId);
            var fl = MaterialsManagementService.GetFolders(pid, subjectid);
            var folders = fl.Select(e => new FoldersViewData(e)).ToList();
            return Ok(folders);
        }

        [HttpGet("BackspaceFolderMaterials")]
        public IActionResult BackspaceFolder(string Id, string subjectId)
        {
            var id = int.Parse(Id);
            var subjectid = int.Parse(subjectId);
            var pid = MaterialsManagementService.GetPidById(id);
            var fl = MaterialsManagementService.GetFolders(pid, subjectid);
            var folders = fl.Select(e => new FoldersViewData(e)).ToList();
            return Ok(folders);
        }

        [HttpPost("CreateFolderMaterials")]
        public IActionResult CreateFolder(string Pid, string subjectId)
        {
            try
            {
                var pid = int.Parse(Pid);
                var subjectid = int.Parse(subjectId);
                var fls = MaterialsManagementService.CreateFolder(pid, subjectid);
                var fl = MaterialsManagementService.GetFolders(pid, subjectid);
                var folders = fl.Select(e => new FoldersViewData(e)).ToList();
                return Ok(folders);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpDelete("DeleteFolderMaterials")]
        public IActionResult DeleteFolder(string IdFolder)
        {
            try
            {
                var idfolder = int.Parse(IdFolder);
                MaterialsManagementService.DeleteFolder(idfolder);
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpDelete("DeleteDocumentMaterials")]
        public IActionResult DeleteDocument(string IdDocument, string pid, string subjectId)
        {
            try
            {
                var iddocument = int.Parse(IdDocument);
                var parentIdFolder = int.Parse(pid);
                var subjectid = int.Parse(subjectId);
                MaterialsManagementService.DeleteDocument(iddocument);
                var fl = MaterialsManagementService.GetDocumentsByIdFolders(parentIdFolder, subjectid);
                var documents = fl.Select(e => new DocumentsViewData(e)).ToList();
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("RenameFolderMaterials")]
        public IActionResult RenameFolder(string id, string pid, string newName, string subjectId)
        {
            try
            {
                var idfolder = int.Parse(id);
                var parentIdFolder = int.Parse(pid);
                var subjectid = int.Parse(subjectId);
                var name = newName;
                MaterialsManagementService.RenameFolder(idfolder, name);
                var fl = MaterialsManagementService.GetFolders(parentIdFolder, subjectid);
                var folders = fl.Select(e => new FoldersViewData(e)).ToList();
                return Ok(folders);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("RenameDocumentMaterials")]
        public IActionResult RenameDocument(string id, string pid, string newName, string subjectId)
        {
            try
            {
                var iddocument = int.Parse(id);
                var parentIdFolder = int.Parse(pid);
                var subjectid = int.Parse(subjectId);
                var name = newName;
                MaterialsManagementService.RenameDocument(iddocument, name);
                var fl = MaterialsManagementService.GetDocumentsByIdFolders(parentIdFolder, subjectid);
                var documents = fl.Select(e => new DocumentsViewData(e)).ToList();
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("SaveTextMaterials")]
        public IActionResult SaveTextMaterials(string idd, string idf, string name, string text, string subjectId)
        {
            var id_document = int.Parse(idd);
            var id_folder = int.Parse(idf);
            var subjectid = int.Parse(subjectId);
            MaterialsManagementService.SaveTextMaterials(id_document, id_folder, name, text, subjectid);
            return Ok();
        }

        [HttpGet("GetDocumentsMaterials")]
        public IActionResult GetDocuments(string Pid, string subjectId)
        {
            var id = int.Parse(Pid);
            var subjectid = int.Parse(subjectId);
            var dc = MaterialsManagementService.GetDocumentsByIdFolders(id, subjectid);
            var documents = dc.Select(e => new DocumentsViewData(e)).ToList();
            return Ok(documents);
        }

        [HttpGet("GetTextMaterials")]
        public IActionResult GetText(string IdDocument)
        {
            var id = int.Parse(IdDocument);
            var document = MaterialsManagementService.GetTextById(id);
            var result = new DocumentsViewData(document);
            return Ok(result);
        }
    }
}