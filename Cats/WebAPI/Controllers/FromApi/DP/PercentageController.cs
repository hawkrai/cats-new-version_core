using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.DPManagement;
using Application.Infrastructure.DTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PercentageController : ApiRoutedController
    {
        private readonly LazyDependency<IPercentageGraphService> _percentageService =
            new LazyDependency<IPercentageGraphService>();

        private IPercentageGraphService PercentageService => _percentageService.Value;

        [HttpGet]
        public IActionResult Get([ModelBinder] GetPagedListParams parms)
        {
            var result = PercentageService.GetPercentageGraphs( /*todo #auth WebSecurity.CurrentUserId*/2, parms);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var result = PercentageService.GetPercentageGraph(id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] PercentageGraphData percentage)
        {
            return SavePercentage(percentage);
        }

        [HttpPut]
        public IActionResult Put([FromBody] PercentageGraphData percentage)
        {
            return SavePercentage(percentage);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            PercentageService.DeletePercentage( /*todo #auth WebSecurity.CurrentUserId*/2, id);

            return Ok();
        }

        private IActionResult SavePercentage(PercentageGraphData percentage)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            PercentageService.SavePercentage( /*todo #auth WebSecurity.CurrentUserId*/2, percentage);
            return Ok();
        }
    }
}