using Application.Core;
using Application.Infrastructure.DPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StudentMarkController : ApiRoutedController
    {
        private readonly LazyDependency<IDpManagementService> _diplomProjectManagementService =
            new LazyDependency<IDpManagementService>();

        private IDpManagementService DpManagementService => _diplomProjectManagementService.Value;

        [HttpPost]
        public IActionResult Post([FromBody] int[] mark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DpManagementService.SetStudentDiplomMark( /*todo #auth WebSecurity.CurrentUserId*/2, mark[0], mark[1]);
            return Ok();
        }
    }
}