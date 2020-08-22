using Application.Core;
using Application.Infrastructure.CPManagement;
using Application.Infrastructure.CTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseProjectConsultationDateController : ApiRoutedController
    {
        private readonly LazyDependency<ICpPercentageGraphService> _percentageService =
            new LazyDependency<ICpPercentageGraphService>();

        private ICpPercentageGraphService PercentageService => _percentageService.Value;

        [HttpPost]
        public IActionResult Post(
            [FromBody] /*DateTime consultationDate, int subject*/CourseProjectConsultationDateData consultationDate)
        {
            PercentageService.SaveConsultationDate( /*todo #auth WebSecurity.CurrentUserId*/2, consultationDate.Day,
                consultationDate.SubjectId);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            PercentageService.DeleteConsultationDate( /*todo #auth WebSecurity.CurrentUserId*/2, id);
            return Ok();
        }
    }
}