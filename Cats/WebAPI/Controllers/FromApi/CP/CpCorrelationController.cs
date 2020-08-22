using Application.Core;
using Application.Infrastructure.CPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CpCorrelationController : ApiRoutedController
    {
        private readonly LazyDependency<ICpCorrelationService> correlationService =
            new LazyDependency<ICpCorrelationService>();

        private ICpCorrelationService CorrelationService => correlationService.Value;

        // GET api/<controller>
        [HttpGet]
        public IActionResult Get(string entity, string subjectId)
        {
            var result = CorrelationService.GetCorrelation(entity,
                subjectId == null ? 0 : int.Parse(subjectId), /*todo #auth WebSecurity.CurrentUserId*/2);
            return Ok(result);
        }
    }
}