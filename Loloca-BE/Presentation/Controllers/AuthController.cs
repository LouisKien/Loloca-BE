using Loloca_BE.Business.Models;
using Loloca_BE.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService accountsService)
        {
            _authService = accountsService;
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/auth")]
        public async Task <IActionResult> Login([FromBody] AccountsView loginInfo)
        {
            if (loginInfo.Email.IsNullOrEmpty())
            {
                return BadRequest("Username is required");
            }
            else if (loginInfo.Password.IsNullOrEmpty())
            {
                return BadRequest("Password is required");
            }
            IActionResult response = Unauthorized();
            var account_ = await _authService.AuthenticateUser(loginInfo);
            if (account_.email == null && account_.code == null)
            {
                return NotFound("No account found");
            }
            //var token = await _accountsService.GenerateTokens(account_);
            //response = Ok(new { accessToken = token.accessToken, refreshToken = token.refreshToken });
            return Ok("Please check your email to verify it is you");
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/verify")]
        public async Task<IActionResult> VerifyLogin([FromBody] EmailVerificationView body)
        {
            if(body.Email.IsNullOrEmpty())
            {
                return BadRequest("Email cannot be empty");
            }
            if (body.Code.IsNullOrEmpty())
            {
                return BadRequest("Verification code cannot be empty");
            }
            IActionResult response = Unauthorized();
            var token = await _authService.GenerateTokens(body.Email, body.Code);
            response = Ok(new { accessToken = token.accessToken, refreshToken = token.refreshToken });
            return response;
        }
    }
}
