using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.CPManagement;
using Application.Infrastructure.CTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseProjectController : ApiRoutedController
    {
        private readonly LazyDependency<ICPManagementService> _cpManagementService =
            new LazyDependency<ICPManagementService>();

        private ICPManagementService CpManagementService => _cpManagementService.Value;

        [HttpGet]
        public IActionResult Get([ModelBinder] GetPagedListParams parms)
        {
            var result = CpManagementService.GetProjects( /*todo #auth WebSecurity.CurrentUserId*/2, parms);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var result = CpManagementService.GetProject(id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CourseProjectData project)
        {
            return SaveProject(project);
        }

        [HttpPut]
        public IActionResult Put([FromBody] CourseProjectData project)
        {
            return SaveProject(project);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            CpManagementService.DeleteProject( /*todo #auth WebSecurity.CurrentUserId*/2, id);
            return Ok();
        }

        private IActionResult SaveProject(CourseProjectData project)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            project.LecturerId = /*todo #auth WebSecurity.CurrentUserId*/2;

            CpManagementService.SaveProject(project);
            return Ok();
        }
    }
}