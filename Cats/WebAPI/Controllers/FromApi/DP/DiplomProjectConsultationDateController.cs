using System;
using Application.Core;
using Application.Infrastructure.DPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DiplomProjectConsultationDateController : ApiRoutedController
    {
        private readonly LazyDependency<IPercentageGraphService> _percentageService =
            new LazyDependency<IPercentageGraphService>();

        private IPercentageGraphService PercentageService => _percentageService.Value;

        [HttpPost("{consultationDate:datetime}")]
        public IActionResult Post(DateTime consultationDate)
        {
            if (ModelState.IsValid)
            {
                PercentageService.SaveConsultationDate( /*todo #auth WebSecurity.CurrentUserId*/2, consultationDate);
                return Ok();
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            PercentageService.DeleteConsultationDate( /*todo #auth WebSecurity.CurrentUserId*/2, id);

            return Ok();
        }
    }
}