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
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountsService;

        public AccountsController(IAccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/accounts/authenticate")]
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
            var account_ = await _accountsService.AuthenticateUser(loginInfo);
            if (account_ == null)
            {
                return NotFound("No account found");
            }
            var token = await _accountsService.GenerateTokens(account_);
            response = Ok(new { accessToken = token.accessToken, refreshToken = token.refreshToken });
            return response;
        }

    }
}
