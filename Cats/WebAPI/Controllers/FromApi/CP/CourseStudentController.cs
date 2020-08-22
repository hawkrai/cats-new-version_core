using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.CPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseStudentController : ApiRoutedController
    {
        private readonly LazyDependency<ICPManagementService> cpManagementService =
            new LazyDependency<ICPManagementService>();

        private ICPManagementService CpManagementService => cpManagementService.Value;

        [HttpGet]
        public IActionResult Get([ModelBinder] GetPagedListParams parms)
        {
            var result = CpManagementService.GetStudentsByCourseProjectId(parms);
            return Ok(result);
        }
    }
}