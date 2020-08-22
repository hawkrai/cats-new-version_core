using System;
using System.Linq;
using Application.Core;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.SubjectManagement;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.Parental;

namespace WebAPI.Controllers.Services.Parental
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ParentalServiceController : ApiRoutedController
    {
        private readonly LazyDependency<IGroupManagementService> groupManagementService =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public IGroupManagementService GroupManagementService => groupManagementService.Value;

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        [HttpGet("GetGroupSubjects/{groupId}")]
        public IActionResult GetGroupSubjects(string groupId)
        {
            try
            {
                var group = int.Parse(groupId);
                var model = SubjectManagementService.GetGroupSubjects(group);

                var result = model.Select(e => new SubjectViewData(e)).ToList();

                return Ok(result);
            }
            catch (Exception e)
            {
                return ServerError500(e.Message);
            }
        }
    }
}