using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     <see cref="ControllerBase"/> class with [<see cref="ApiControllerAttribute"/>] and [<see cref="RouteAttribute"/>("api/[controller]")].
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ApiRoutedController : ControllerBase
    {
        /// <summary>
        ///     Implements <see cref="HttpStatusCode"/> InternalServerError result.
        /// </summary>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <param name="data">Data object.</param>
        /// <returns><see cref="object"/> with controller name and error data.</returns>
        public ObjectResult ServerError500<T>(T data)
        {
            var controllerName = this.GetType().Name;

            var response = new
            {
                controllerName,
                data
            };

            var result = StatusCode((int) HttpStatusCode.InternalServerError, response);

            return result;
        }
    }
}