using BookLending.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookLending.API.Controllers
{
    [Route("errors/{statusCode}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class ErrorsController : ControllerBase
    {
        public IActionResult Error(int statusCode)
        {
            var response = new ApiErrorResponse(statusCode);

            return statusCode switch
            {
                400 => BadRequest(response),
                401 => Unauthorized(response),
                403 => Forbid(),
                404 => NotFound(response),
                500 => StatusCode(500, response),
                _ => StatusCode(statusCode, response)
            };
        }
    }
}
