using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.CPManagement;
using Application.Infrastructure.CTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseProjectConsultationController : ApiRoutedController
    {
        private readonly LazyDependency<ICPManagementService> _courseProjectManagementService =
            new LazyDependency<ICPManagementService>();

        private readonly LazyDependency<ICpPercentageGraphService> _percentageService =
            new LazyDependency<ICpPercentageGraphService>();

        private ICPManagementService CpManagementService => _courseProjectManagementService.Value;

        private ICpPercentageGraphService PercentageService => _percentageService.Value;

        [HttpGet]
        public IActionResult Get([ModelBinder] GetPagedListParams parms)
        {
            var lecturerId = /*todo #auth WebSecurity.CurrentUserId*/2;
            if (parms.Filters.ContainsKey("lecturerId"))
            {
                lecturerId = int.Parse(parms.Filters["lecturerId"]);
            }

            var subjectId = 0;
            if (parms.Filters.ContainsKey("subjectId"))
            {
                subjectId = int.Parse(parms.Filters["subjectId"]);
            }

            var groupId = 0;
            if (parms.Filters.ContainsKey("groupId"))
            {
                groupId = int.Parse(parms.Filters["groupId"]);
            }

            var result = new
            {
                Students =
                    CpManagementService.GetGraduateStudentsForGroup(lecturerId, groupId, subjectId, parms, false),
                CourseProjectConsultationDates = PercentageService.GetConsultationDatesForUser(lecturerId, subjectId)
            };

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CourseProjectConsultationMarkData consultationMark)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            PercentageService.SaveConsultationMark( /*todo #auth WebSecurity.CurrentUserId*/2, consultationMark);
            return Ok();
        }
    }
}