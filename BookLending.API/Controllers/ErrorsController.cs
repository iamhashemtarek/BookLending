using BookLending.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookLending.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class ErrorsController : ControllerBase
    {
        [Route("errors/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            return StatusCode(statusCode, new ApiErrorResponse(statusCode));
        }
    }
}
