using System;
using Application.Core;
using Application.Infrastructure.DPManagement;
using LMP.Models.DP;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DiplomProjectNewsController : ApiRoutedController
    {
        private readonly LazyDependency<IDpManagementService> _dpManagementService =
            new LazyDependency<IDpManagementService>();

        private IDpManagementService DpManagementService => _dpManagementService.Value;

        [HttpGet]
        public IActionResult Get()
        {
            var result = DpManagementService.GetNewses( /*todo #auth WebSecurity.CurrentUserId*/2);
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] DiplomProjectNews deleteData)
        {
            try
            {
                DpManagementService.DeleteNews(deleteData);
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }
    }
}