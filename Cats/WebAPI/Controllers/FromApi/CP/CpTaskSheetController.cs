using Application.Core;
using Application.Infrastructure.CPManagement;
using Application.Infrastructure.CTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CpTaskSheetController : ApiRoutedController
    {
        private readonly LazyDependency<ICPManagementService> _courseProjectManagementService =
            new LazyDependency<ICPManagementService>();

        private ICPManagementService CpManagementService => _courseProjectManagementService.Value;

        [HttpGet("{courseProjectId:int}")]
        public IActionResult Get(int courseProjectId)
        {
            var result = CpManagementService.GetTaskSheet(courseProjectId);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TaskSheetData taskSheet)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            CpManagementService.SaveTaskSheet( /*todo #auth WebSecurity.CurrentUserId*/2, taskSheet);
            return Ok();
        }
    }
}