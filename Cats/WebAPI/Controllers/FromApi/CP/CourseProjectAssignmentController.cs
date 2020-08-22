using Application.Core;
using Application.Infrastructure.CPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseProjectAssignmentController : ApiRoutedController
    {
        private readonly LazyDependency<ICPManagementService> _cpManagementService =
            new LazyDependency<ICPManagementService>();

        private ICPManagementService CpManagementService => _cpManagementService.Value;

        [HttpPost]
        public IActionResult Post([FromBody] AssignProjectUpdateModel updateModel)
        {
            CpManagementService.AssignProject( /*todo #auth WebSecurity.CurrentUserId*/2, updateModel.ProjectId,
                updateModel.StudentId);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            CpManagementService.DeleteAssignment( /*todo #auth WebSecurity.CurrentUserId*/2, id);

            return Ok();
        }

        public class AssignProjectUpdateModel
        {
            public int ProjectId { get; set; }

            public int StudentId { get; set; }
        }
    }
}