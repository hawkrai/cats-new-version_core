using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.DPManagement;
using Application.Infrastructure.DTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DiplomProjectController : ApiRoutedController
    {
        private readonly LazyDependency<IDpManagementService> _dpManagementService =
            new LazyDependency<IDpManagementService>();

        private IDpManagementService DpManagementService => _dpManagementService.Value;

        [HttpGet]
        public IActionResult Get([ModelBinder] GetPagedListParams parms)
        {
            var result = DpManagementService.GetProjects( /*todo #auth WebSecurity.CurrentUserId*/2, parms);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var result = DpManagementService.GetProject(id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] DiplomProjectData project)
        {
            return SaveProject(project);
        }

        [HttpPut]
        public IActionResult Put([FromBody] DiplomProjectData project)
        {
            return SaveProject(project);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            DpManagementService.DeleteProject( /*todo #auth WebSecurity.CurrentUserId*/2, id);

            return Ok();
        }

        private IActionResult SaveProject(DiplomProjectData project)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            project.LecturerId = /*todo #auth WebSecurity.CurrentUserId*/2;
            DpManagementService.SaveProject(project);
            return Ok();
        }
    }
}