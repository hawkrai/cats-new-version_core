using System;
using System.Linq;
using Application.Core;
using Application.Infrastructure.ProjectManagement;
using LMP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.BTS;

namespace WebAPI.Controllers.Services.BTS
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "student,lector")]
    public class ProjectsServiceController : ApiRoutedController
    {
        private readonly LazyDependency<IProjectManagementService> projectManagementService =
            new LazyDependency<IProjectManagementService>();

        private IProjectManagementService ProjectManagementService => projectManagementService.Value;

        [HttpGet("Index")]
        public ProjectsResult Index(int pageSize, int pageNumber, string sortingPropertyName, bool desc = false,
            string searchString = null)
        {
            var projects = ProjectManagementService.GetUserProjects( /*todo #auth WebSecurity.CurrentUserId*/2,
                    pageSize, pageNumber, sortingPropertyName, desc, searchString)
                .Select(e => new ProjectViewData(e)).ToList();
            var totalCount =
                ProjectManagementService.GetUserProjectsCount( /*todo #auth WebSecurity.CurrentUserId*/2, searchString);
            return new ProjectsResult
            {
                Projects = projects,
                TotalCount = totalCount
            };
        }

        [HttpGet("Show/{id}")]
        public ProjectResult Show(string id, bool withDetails)
        {
            var intId = Convert.ToInt32(id);
            ProjectViewData project;
            if (withDetails)
                project = new ProjectViewData(ProjectManagementService
                    .GetProjectWithData(intId, true), withBugs: true, withMembers: true);
            else
                project = new ProjectViewData(ProjectManagementService.GetProjectWithData(intId));
            return new ProjectResult
            {
                Project = project
            };
        }

        [HttpGet("ProjectParticipationsByUser/{id}")]
        public UserProjectParticipationsResult ProjectParticipationsByUser(string id, int pageSize, int pageNumber,
            string sortingPropertyName, bool desc = false, string searchString = null)
        {
            var convertedUserId = Convert.ToInt32(id);
            var projects = ProjectManagementService.GetUserProjectParticipations(convertedUserId, pageSize, pageNumber,
                    sortingPropertyName, desc, searchString)
                .Select(e => new UserProjectParticipationViewData(e, convertedUserId)).ToList();
            var totalCount = ProjectManagementService.GetUserProjectParticipationsCount(convertedUserId, searchString);
            return new UserProjectParticipationsResult
            {
                Projects = projects,
                TotalCount = totalCount
            };
        }

        [HttpGet("StudentsParticipationsByGroup/{id}")]
        public StudentsParticipationsResult StudentsParticipationsByGroup(string id, int pageSize, int pageNumber)
        {
            var students = ProjectManagementService.GetStudentsGroupParticipations(int.Parse(id), pageSize, pageNumber)
                .Select(e => new StudentParticipationViewData(e)).ToList();
            var totalCount = ProjectManagementService.GetStudentsGroupParticipationsCount(int.Parse(id));
            return new StudentsParticipationsResult
            {
                ProjectsStudents = students,
                TotalCount = totalCount
            };
        }

        [HttpGet("Projects/{id}/Bugs")]
        public ProjectCommentsResult GetProjectComments(string id)
        {
            var comments = ProjectManagementService.GetProjectComments(int.Parse(id))
                .Select(e => new ProjectCommentViewData(e)).ToList();
            return new ProjectCommentsResult
            {
                Comments = comments
            };
        }

        [HttpPost("Projects/{id}/SaveFile")]
        public ProjectFileResult SaveFile(string id, ProjectFileViewData projectFile)
        {
            var attachment = new Attachment
            {
                Name = projectFile.Name,
                FileName = projectFile.FileName,
                AttachmentType = (AttachmentType) Enum.Parse(typeof(AttachmentType), projectFile.AttachmentType)
            };
            attachment = ProjectManagementService.SaveAttachment(int.Parse(id), attachment);
            return new ProjectFileResult
            {
                ProjectFile = new ProjectFileViewData(attachment)
            };
        }

        [HttpGet("Projects/{id}/Attachments")]
        public ProjectFilesResult GetAttachments(string id)
        {
            var attachments = ProjectManagementService.GetAttachments(int.Parse(id));
            return new ProjectFilesResult
            {
                ProjectFiles = attachments.Select(e => new ProjectFileViewData(e)).ToList()
            };
        }

        [HttpDelete("Projects/{id}/Attachments/{fileName}")]
        public void DeleteAttachment(string id, string fileName)
        {
            ProjectManagementService.DeleteAttachment(int.Parse(id), fileName);
        }

        //todo unused
        #region unused

        //[HttpPost("/Projects/{id}/GenerateMatrix")]
        //public void GenerateMatrix(string id, ProjectMatrixViewData matrix)
        //{
        //    var projectId = int.Parse(id);
        //    MatrixManagmentService.ClearRequirements(projectId);
        //    MatrixManagmentService.FillRequirements(projectId, matrix.RequirementsFileName);
        //    MatrixManagmentService.FillRequirementsCoverage(projectId, matrix.TestsFileName);
        //}

        //[HttpGet("/Projects/{id}/Matrix")]
        //public ProjectMatrixResult GetMatrix(string id)
        //{
        //    var requirements = MatrixManagmentService.GetRequirements(int.Parse(id)).Select(e => new ProjectMatrixRequirementViewData(e)).ToList();
        //    return new ProjectMatrixResult()
        //    {
        //        Project = new ProjectMatrixViewData()
        //        {
        //            Requirements = requirements
        //        }
        //    };
        //}

        //private IMatrixManagmentService MatrixManagmentService => matrixManagementService.Value;
        //private readonly LazyDependency<IMatrixManagmentService> matrixManagementService = new LazyDependency<IMatrixManagmentService>();

        #endregion
    }
}