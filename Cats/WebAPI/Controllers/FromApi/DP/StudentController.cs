using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.DPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
    public class StudentController : ApiRoutedController
    {
        private readonly LazyDependency<IDpManagementService> dpManagementService =
            new LazyDependency<IDpManagementService>();

        private IDpManagementService DpManagementService => dpManagementService.Value;

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpGet]
        public IActionResult Get([ModelBinder] GetPagedListParams parms)
        {
            var result = DpManagementService.GetStudentsByDiplomProjectId(parms);
            return Ok(result);
        }
    }
}