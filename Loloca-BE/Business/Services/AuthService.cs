using AutoMapper;
using Loloca_BE.Business.Models.AccountsView;
using Loloca_BE.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Loloca_BE.Business.Services
{
    public class AuthService : IAuthService
    {
        // Hard data, change it to your own for testing
        private string email = "louisnamu02@gmail.com";
        private string password = "string";

        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _memoryCache;

        public AuthService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _memoryCache = memoryCache;
        }

        public async Task<(string? email, string? code)> AuthenticateUser(AccountsView loginInfo)
        {
            string? emailReturn = null;
            string? codeReturn = null;

            try
            {
                //string hashedPassword = await HashPassword(loginInfo.Password);

                var account = new AccountsView
                {
                    Email = email,
                    Password = password,
                    Role = "Admin"
                };
                if (account != null && loginInfo.Email == account.Email && loginInfo.Password == account.Password)
                {
                    emailReturn = loginInfo.Email;
                    codeReturn = GenerateVerificationCode();

                    _memoryCache.Set(loginInfo.Email, codeReturn, TimeSpan.FromMinutes(60));

                    // Send verification email
                    await _emailService.SendVerificationEmailAsync(loginInfo.Email, codeReturn);
                }
                return (emailReturn, codeReturn);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public Task<string?> HashPassword(string password)
        {
            try
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        stringBuilder.Append(hashBytes[i].ToString("x2"));
                    }

                    return Task.FromResult<string?>(stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(string accessToken, string refreshToken)> GenerateTokens(string email, string verificationCode)
        {
            // Check if the code exists in the cache and is not expired
            if (_memoryCache.TryGetValue(email, out string? cachedCode) && cachedCode == verificationCode)
            {
                try
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var account = new AccountsView
                    {
                        Email = email,
                        Role = "Admin"
                    };

                    var accessClaims = new List<Claim>
                    {
                        new Claim("Role", account.Role.ToString()),
                        new Claim("Email", account.Email.ToString())
                    };

                    var accessExpiration = DateTime.UtcNow.AddHours(1);
                    var accessJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], accessClaims, expires: accessExpiration, signingCredentials: credentials);
                    var accessToken = new JwtSecurityTokenHandler().WriteToken(accessJwt);

                    var refreshClaims = new List<Claim>
                    {
                        new Claim("Email", account.Email)
                    };
                    var refreshExpiration = DateTime.UtcNow.AddDays(14);
                    var refreshJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], refreshClaims, expires: refreshExpiration, signingCredentials: credentials);
                    var refreshToken = new JwtSecurityTokenHandler().WriteToken(refreshJwt);

                    // Store refresh token in the database
                    //var token = new Token
                    //{
                    //    AccountId = accounts.Id,
                    //    RefreshToken = refreshToken,
                    //    ExpiredDate = refreshExpiration
                    //};

                    //_unitOfWork.TokenRepository.Insert(token);
                    //_unitOfWork.Save();

                    _memoryCache.Remove(email);

                    return (accessToken, refreshToken);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                // Invalid code or code expired
                throw new Exception("Invalid verification code.");
            }
        }
    }
}
