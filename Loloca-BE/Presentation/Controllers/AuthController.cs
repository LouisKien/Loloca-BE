using Loloca_BE.Business.Models;
using Loloca_BE.Business.Models.AccountView;
using Loloca_BE.Business.Models.RefreshTokenView;
using Loloca_BE.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;

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
        public async Task <IActionResult> Login([FromBody] AuthRequest loginInfo)
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
            if(account_ != null)
            {
                switch (account_.Status)
                {
                    case 0:
                        return BadRequest("Your account is locked by administrator");
                    case 1:
                        if(account_.Role == 1 || account_.Role == 2)
                        {
                            bool check = await _authService.AuthenticateUserAdvanced(account_);
                            if (check)
                            {
                                return Ok("Email has been sent, please check email to verify your account, if you don't see it, check your spam");
                            } else
                            {
                                return BadRequest("Something went wrong");
                            }
                        }
                        else if(account_.Role == 3)
                        {
                            var token = await _authService.GenerateTokens(account_.Email);
                            if(token.refreshToken.IsNullOrEmpty() || token.accessToken.IsNullOrEmpty())
                            {
                                return BadRequest("Something went wrong");
                            }
                            response = Ok(new { accessToken = token.accessToken, refreshToken = token.refreshToken });
                            return response;
                        }
                        break;
                    case 2:
                        await _authService.SendVerificationEmail(account_.Email);
                        return Ok("Your account hasn't verified yet, please check email to verify your account, if you don't see it, check your spam");
                    default:
                        return BadRequest("Something went wrong");
                }
            }
            return NotFound("Wrong email or password");
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/auth/verify")]
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
            if (token.accessToken.IsNullOrEmpty() || token.refreshToken.IsNullOrEmpty())
            {
                response = BadRequest("Something went wrong");
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/auth/refresh")]
        public async Task<IActionResult> AccessTokenRequest([FromBody] RefreshTokenRequest refreshToken)
        {
            IActionResult response = Unauthorized();
            var token = await _authService.RefreshingAccessToken(refreshToken.RefreshToken);
            response = Ok(new { accessToken = token.accessToken, refreshToken = token.refreshToken });
            if (token.accessToken.IsNullOrEmpty() || token.refreshToken.IsNullOrEmpty())
            {
                response = BadRequest("Something went wrong");
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest body)
        {
            if (body.Email.IsNullOrEmpty())
            {
                return BadRequest("Email cannot be empty");
            }
            if(await _authService.CheckExistedEmail(body.Email))
            {
                return BadRequest("Your email has already existed");
            }
            if (body.Password.IsNullOrEmpty())
            {
                return BadRequest("Password cannot be empty");
            }
            if (body.FirstName.IsNullOrEmpty())
            {
                return BadRequest("First name cannot be empty");
            }
            if (body.LastName.IsNullOrEmpty())
            {
                return BadRequest("Last name cannot be empty");
            }
            if (!body.Gender.HasValue)
            {
                return BadRequest("Gender cannot be empty");
            }
            if (body.PhoneNumber.IsNullOrEmpty())
            {
                return BadRequest("First name cannot be empty");
            }
            if (!body.DateOfBirth.HasValue)
            {
                return BadRequest("Date of Birth cannot be empty");
            }
            bool check = await _authService.RegisterCustomer(body);
            if (check)
            {
                await _authService.SendVerificationEmail(body.Email);
                return Ok("Create success, please check your email to verify your account, if you don't see it, check your spam");
            }
            return BadRequest("Something went wrong");
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/register/tourguide")]
        public async Task<IActionResult> RegisterTourGuide([FromBody] RegisterTourGuideRequest body)
        {
            if (body.Email.IsNullOrEmpty())
            {
                return BadRequest("Email cannot be empty");
            }
            if (await _authService.CheckExistedEmail(body.Email))
            {
                return BadRequest("Your email has already existed");
            }
            if (body.Password.IsNullOrEmpty())
            {
                return BadRequest("Password cannot be empty");
            }
            if (body.FirstName.IsNullOrEmpty())
            {
                return BadRequest("First name cannot be empty");
            }
            if (body.LastName.IsNullOrEmpty())
            {
                return BadRequest("Last name cannot be empty");
            }
            if (!body.Gender.HasValue)
            {
                return BadRequest("Gender cannot be empty");
            }
            if (body.PhoneNumber.IsNullOrEmpty())
            {
                return BadRequest("First name cannot be empty");
            }
            if (body.Address.IsNullOrEmpty())
            {
                return BadRequest("Address cannot be empty");
            }
            if (!body.DateOfBirth.HasValue)
            {
                return BadRequest("Date of Birth cannot be empty");
            }
            bool check = await _authService.RegisterTourGuide(body);
            if (check)
            {
                await _authService.SendVerificationEmail(body.Email);
                return Ok("Create success, please check your email to verify your account, if you don't see it, check your spam");
            }
            return BadRequest("Something went wrong");
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/register/verify")]
        public async Task<IActionResult> VerifyRegister([FromBody] EmailVerificationView body)
        {
            if (body.Email.IsNullOrEmpty())
            {
                return BadRequest("Email cannot be empty");
            }
            if (body.Code.IsNullOrEmpty())
            {
                return BadRequest("Verification code cannot be empty");
            }
            bool check = await _authService.VerifyAccount(body.Email, body.Code);
            if (check)
            {
                IActionResult response = Unauthorized();
                var token = await _authService.GenerateTokens(body.Email);
                response = Ok(new { accessToken = token.accessToken, refreshToken = token.refreshToken });
                if (token.accessToken.IsNullOrEmpty() || token.refreshToken.IsNullOrEmpty())
                {
                    response = BadRequest("Something went wrong");
                }
                return response;
            }
            else
            {
                return BadRequest("Invalid verification code, if you don't see it, check your spam");
            }
        }
    }
}
