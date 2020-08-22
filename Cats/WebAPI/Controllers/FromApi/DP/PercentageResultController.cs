using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.DPManagement;
using Application.Infrastructure.DTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PercentageResultController : ApiRoutedController
    {
        private readonly LazyDependency<IDpManagementService> _diplomProjectManagementService =
            new LazyDependency<IDpManagementService>();

        private readonly LazyDependency<IPercentageGraphService> _percentageService =
            new LazyDependency<IPercentageGraphService>();

        private IPercentageGraphService PercentageService => _percentageService.Value;

        private IDpManagementService DpManagementService => _diplomProjectManagementService.Value;

        [HttpGet]
        public IActionResult Get([ModelBinder] GetPagedListParams parms)
        {
            var result = new
            {
                Students = DpManagementService.GetGraduateStudentsForUser( /*todo #auth WebSecurity.CurrentUserId*/2,
                    parms),
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

        [HttpPut]
        public IActionResult Put([FromBody] PercentageResultData percentage)
        {
            return SavePercentageResult(percentage);
        }

        private IActionResult SavePercentageResult(PercentageResultData percentageResult)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            PercentageService.SavePercentageResult( /*todo #auth WebSecurity.CurrentUserId*/2, percentageResult);
            return Ok();
        }
    }
}