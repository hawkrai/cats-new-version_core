using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.DPManagement;
using Application.Infrastructure.DTO;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DipomProjectConsultationController : ApiRoutedController
    {
        private readonly LazyDependency<IDpManagementService> _diplomProjectManagementService =
            new LazyDependency<IDpManagementService>();

        private readonly LazyDependency<IPercentageGraphService> _percentageService =
            new LazyDependency<IPercentageGraphService>();

        private IDpManagementService DpManagementService => _diplomProjectManagementService.Value;

        private IPercentageGraphService PercentageService => _percentageService.Value;

        [HttpGet]
        public IActionResult Get([ModelBinder] GetPagedListParams parms)
        {
            var lecturerId = /*todo #auth WebSecurity.CurrentUserId*/2;
            if (parms.Filters.ContainsKey("lecturerId")) lecturerId = int.Parse(parms.Filters["lecturerId"]);

            var result = new
            {
                Students = DpManagementService.GetGraduateStudentsForUser(lecturerId, parms, false),
                DipomProjectConsultationDates = PercentageService.GetConsultationDatesForUser(lecturerId)
            };

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] DipomProjectConsultationMarkData consultationMark)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PercentageService.SaveConsultationMark( /*todo #auth WebSecurity.CurrentUserId*/2, consultationMark);
            return Ok();
        }
    }
}