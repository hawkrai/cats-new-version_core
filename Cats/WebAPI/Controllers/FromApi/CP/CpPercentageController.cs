using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.CPManagement;
using Application.Infrastructure.CTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CpPercentageController : ApiRoutedController
    {
        private readonly LazyDependency<ICpPercentageGraphService> _percentageService =
            new LazyDependency<ICpPercentageGraphService>();

        private ICpPercentageGraphService PercentageService => _percentageService.Value;

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