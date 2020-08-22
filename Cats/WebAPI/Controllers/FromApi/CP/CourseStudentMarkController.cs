using Application.Core;
using Application.Infrastructure.CPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseStudentMarkController : ApiRoutedController
    {
        private readonly LazyDependency<ICPManagementService> _courseProjectManagementService =
            new LazyDependency<ICPManagementService>();

        private ICPManagementService CpManagementService => _courseProjectManagementService.Value;

        [HttpPost]
        public IActionResult Post([FromBody] int[] mark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CpManagementService.SetStudentDiplomMark( /*todo #auth WebSecurity.CurrentUserId*/2, mark[0], mark[1]);
            return Ok();
        }
    }
}