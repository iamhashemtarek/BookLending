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
            return NotFound(new ApiErrorResponse(statusCode));
        }
    }
}
