using Application.Core;
using Application.Infrastructure.DPManagement;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.DP
{
	/// <summary>
    ///  Ok
    /// </summary>
    public class UserController : ApiRoutedController
    {
        private readonly LazyDependency<IUserService> userService = new LazyDependency<IUserService>();

        private IUserService UserService => userService.Value;

        /// <summary>
        ///     Ok
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            var result = UserService.GetUserInfo( /*todo #auth WebSecurity.CurrentUserId*/2);

            return Ok(result);
        }
    }
}