using Application.Core;
using Application.Infrastructure.DPManagement;
using Application.Infrastructure.DTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TaskSheetController : ApiRoutedController
    {
        private readonly LazyDependency<IDpManagementService> _diplomProjectManagementService =
            new LazyDependency<IDpManagementService>();

        private IDpManagementService DpManagementService => _diplomProjectManagementService.Value;

        [HttpGet("{id:int}")]
        public IActionResult Get(int diplomProjectId)
        {
            var result = DpManagementService.GetTaskSheet(diplomProjectId);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TaskSheetData taskSheet)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            DpManagementService.SaveTaskSheet( /*todo #auth WebSecurity.CurrentUserId*/2, taskSheet);
            return Ok();
        }
    }
}