using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.CPManagement;
using Application.Infrastructure.CTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CpPercentageResultController : ApiRoutedController
    {
        private readonly LazyDependency<ICPManagementService> _courseProjectManagementService =
            new LazyDependency<ICPManagementService>();

        private readonly LazyDependency<ICpPercentageGraphService> _percentageService =
            new LazyDependency<ICpPercentageGraphService>();

        private ICpPercentageGraphService PercentageService => _percentageService.Value;

        private ICPManagementService CpManagementService => _courseProjectManagementService.Value;

        [HttpGet]
        public IActionResult Get([ModelBinder] GetPagedListParams parms)
        {
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
                Students = CpManagementService.GetGraduateStudentsForGroup( /*todo #auth WebSecurity.CurrentUserId*/2,
                    groupId,
                    subjectId, parms, false),
                PercentageGraphs =
                    PercentageService.GetPercentageGraphsForLecturerAll( /*todo #auth WebSecurity.CurrentUserId*/2,
                        parms)
            };
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] PercentageResultData percentage)
        {
            return SavePercentageResult(percentage);
        }

        [HttpPost]
        public IActionResult Put([FromBody] PercentageResultData percentage)
        {
            return SavePercentageResult(percentage);
        }

        private IActionResult SavePercentageResult(PercentageResultData percentageResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PercentageService.SavePercentageResult( /*todo #auth WebSecurity.CurrentUserId*/2, percentageResult);
            return Ok();
        }
    }
}