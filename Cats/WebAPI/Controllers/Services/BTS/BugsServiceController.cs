using System;
using System.Linq;
using Application.Core;
using Application.Infrastructure.BugManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.BTS;

namespace WebAPI.Controllers.Services.BTS
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "student,lector")]
    public class BugsServiceController : ApiRoutedController
    {
        private readonly LazyDependency<IBugManagementService> bugManagementService =
            new LazyDependency<IBugManagementService>();

        private IBugManagementService BugManagementService => bugManagementService.Value;

        [HttpGet("Index")]
        public BugsResult Index(int pageSize, int pageNumber, string sortingPropertyName, bool desc = false,
            string searchString = null)
        {
            var bugs = BugManagementService.GetUserBugs( /*todo #auth WebSecurity.CurrentUserId*/2, pageSize,
                    pageNumber, sortingPropertyName, desc, searchString)
                .Select(e => new BugViewData(e)).ToList();
            var totalCount =
                BugManagementService.GetUserBugsCount( /*todo #auth WebSecurity.CurrentUserId*/2, searchString);
            return new BugsResult
            {
                Bugs = bugs,
                TotalCount = totalCount
            };
        }

        [HttpGet("Projects/{projectId}/Index")]
        public BugsResult ProjectBugs(string projectId, int pageSize, int pageNumber, string sortingPropertyName,
            bool desc = false, string searchString = null)
        {
            var convertedProjectId = Convert.ToInt32(projectId);
            var bugs = BugManagementService.GetProjectBugs(convertedProjectId, pageSize, pageNumber,
                    sortingPropertyName, desc, searchString)
                .Select(e => new BugViewData(e)).ToList();
            var totalCount = BugManagementService.GetProjectBugsCount(convertedProjectId, searchString);
            return new BugsResult
            {
                Bugs = bugs,
                TotalCount = totalCount
            };
        }
    }
}