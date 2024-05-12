using AutoMapper;
using Loloca_BE.Business.Models;
using Loloca_BE.Data.Repositories;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Loloca_BE.Business.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AccountsService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AccountsView?> AuthenticateUser(AccountsView loginInfo)
        {
            AccountsView accountsView = null;
            try
            {
                //string hashedPassword = await HashPassword(loginInfo.Password);
                var account = new AccountsView
                {
                    Email = "test@gmail.com",
                    Password = "1",
                    Role = "Admin"
                };
                if (account != null && loginInfo.Email == account.Email && loginInfo.Password == account.Password)
                {
                    accountsView = new AccountsView();
                    accountsView.Email = account.Email;
                    accountsView.Role = account.Role.ToString();
                }
                return accountsView;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<string?> HashPassword(string password)
        {
            try
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    // Compute hash from the password bytes
                    byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                    // Convert the byte array to a hexadecimal string
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

        public async Task<(string accessToken, string refreshToken)> GenerateTokens(AccountsView account)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var accessClaims = new List<Claim>
                {
                    new Claim("Role", account.Role.ToString()),
                    new Claim("Email", account.Email.ToString())
                };

                var accessExpiration = DateTime.UtcNow.AddHours(1);
                var accessJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], accessClaims, expires: accessExpiration, signingCredentials: credentials);
                var accessToken = new JwtSecurityTokenHandler().WriteToken(accessJwt);

                var accounts = new AccountsView
                {
                    Email = "test@gmail.com",
                    Password = "1",
                    Role = "Admin"
                };

                var refreshClaims = new List<Claim>
                {
                    new Claim("Email", accounts.Email)
                };
                var refreshExpiration = DateTime.UtcNow.AddDays(14);
                var refreshJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], refreshClaims, expires: refreshExpiration, signingCredentials: credentials);
                var refreshToken = new JwtSecurityTokenHandler().WriteToken(refreshJwt);

                // Store refresh token in the database
                // For simplicity, let's assume there's a method to store it

                //var token = new Token
                //{
                //    AccountId = accounts.Id,
                //    RefreshToken = refreshToken,
                //    ExpiredDate = refreshExpiration
                //};

                //_unitOfWork.TokenRepository.Insert(token);
                //_unitOfWork.Save();

                return (accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
