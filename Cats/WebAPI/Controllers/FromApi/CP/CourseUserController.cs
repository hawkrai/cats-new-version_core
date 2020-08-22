using Application.Core;
using Application.Infrastructure.CPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseUserController : ApiRoutedController
    {
        private readonly LazyDependency<ICPUserService> _userService = new LazyDependency<ICPUserService>();

        private ICPUserService UserService => _userService.Value;

        [HttpGet]
        public IActionResult Get()
        {
            var result = UserService.GetUserInfo( /*todo #auth WebSecurity.CurrentUserId*/2);
            return Ok(result);
        }
    }
}