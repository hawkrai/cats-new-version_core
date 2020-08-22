using Application.Core;
using Application.Infrastructure.CPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseProjectSubjectController : ApiRoutedController
    {
        private readonly LazyDependency<ICPManagementService> _cpManagementService =
            new LazyDependency<ICPManagementService>();

        private ICPManagementService CPManagementService => _cpManagementService.Value;

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var sub = CPManagementService.GetSubject(id);
            return Ok(sub);
        }
    }
}