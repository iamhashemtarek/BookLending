using BookLending.Application.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Application.Services;
using BookLending.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookLending.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<JwtToken>> Register([FromBody] CreateUserDto request)
        {
            try
            {
                var token = await _accountService.RegisterAsync(request);
                return Ok(new JwtToken { Token = token});
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<JwtToken>> Login([FromBody] LoginDto request)
        {
            try
            {
                var token = await _accountService.LoginAsync(request);
                return Ok(new JwtToken { Token = token });
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiErrorResponse(401, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message));
            }
        }
    }
}
