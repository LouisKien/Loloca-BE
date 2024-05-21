using AutoMapper;
using Loloca_BE.Business.Models.AccountView;
using Loloca_BE.Data.Entities;
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

        public async Task<AuthResponse> AuthenticateUser(AuthRequest loginInfo)
        {
            AuthResponse response = new AuthResponse();
            string hashedPassword = await HashPassword(loginInfo.Password);
            var accounts = await _unitOfWork.AccountRepository.FindAsync(a => a.Email == loginInfo.Email && a.HashedPassword == hashedPassword);
            if (accounts.Any())
            {
                var account = Enumerable.FirstOrDefault(accounts);
                response.AccountId = account.AccountId;
                response.Email = account.Email;
                response.Role = account.Role;
                response.Status = account.Status;
                return response;
            }
            return null;
        }

        public async Task<bool> AuthenticateUserAdvanced(AuthResponse authResponse)
        {
            try
            {
                string emailReturn;
                
                var accounts = await _unitOfWork.AccountRepository.FindAsync(a => a.Email == authResponse.Email);

                if (accounts.Any())
                {
                    var account = Enumerable.FirstOrDefault(accounts);
                    if(account != null)
                    {
                        emailReturn = authResponse.Email;
                        await SendVerificationEmail(emailReturn);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendVerificationEmail(string email)
        {
            try
            {
                // Send verification email
                string verificationCode;
                verificationCode = await GenerateVerificationCode();
                _memoryCache.Set(email, verificationCode, TimeSpan.FromMinutes(60));
                await _emailService.SendVerificationEmailAsync(email, verificationCode);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<String> GenerateVerificationCode()
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
            var accounts = await _unitOfWork.AccountRepository.FindAsync(a => a.Email == email);
            if(accounts.Any())
            {
                var account = accounts.FirstOrDefault();
                if(account != null)
                {
                    // Check if the code exists in the cache and is not expired
                    if (_memoryCache.TryGetValue(email, out string? cachedCode) && cachedCode == verificationCode && account.Status == 1)
                    {
                        try
                        {
                            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


                            var accessClaims = new List<Claim>
                        {
                            new Claim("Role", account.Role.ToString()),
                            new Claim("Email", account.Email)
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
                            var token = new RefreshToken
                            {
                                AccountId = account.AccountId,
                                Token = refreshToken,
                                ExpiredDate = refreshExpiration,
                                Status = true,
                                DeviceName = "Unknown"
                            };

                            await _unitOfWork.RefreshTokenRepository.InsertAsync(token);
                            await _unitOfWork.SaveAsync();

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
                        _memoryCache.Remove(email);
                        throw new Exception("Invalid verification code.");
                    }
                }
            }
            return (null, null);
        }

        public async Task<(string accessToken, string refreshToken)> GenerateTokens(AuthResponse authResponse)
        {
            try
            {
                var accounts = await _unitOfWork.AccountRepository.FindAsync(a => a.Email == authResponse.Email);
                if (accounts.Any())
                {
                    var account = accounts.FirstOrDefault();
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


                    var accessClaims = new List<Claim>
                        {
                            new Claim("Role", account.Role.ToString()),
                            new Claim("Email", account.Email),
                            new Claim("AccountId", account.AccountId.ToString())
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
                    var token = new RefreshToken
                    {
                        AccountId = account.AccountId,
                        Token = refreshToken,
                        ExpiredDate = refreshExpiration,
                        Status = true,
                        DeviceName = "Unknown"
                    };

                    await _unitOfWork.RefreshTokenRepository.InsertAsync(token);
                    await _unitOfWork.SaveAsync();

                    return (accessToken, refreshToken);
                }
                return (null, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> VerifyAccount(string email, string verificationCode)
        {
            if (_memoryCache.TryGetValue(email, out string? cachedCode) && cachedCode == verificationCode)
            {
                using (var transaction = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        var accounts = await _unitOfWork.AccountRepository.FindAsync(a => a.Email == email);
                        if (accounts.Any())
                        {
                            var account = accounts.FirstOrDefault();
                            if (account != null)
                            {
                                if (account.Status == 2)
                                {
                                    account.Status = 1;
                                    await _unitOfWork.AccountRepository.UpdateAsync(account);
                                    await _unitOfWork.SaveAsync();
                                    if (account.Role == 2)
                                    {
                                        var tourGuides = await _unitOfWork.TourGuideRepository.FindAsync(t => t.AccountId == account.AccountId);
                                        if (tourGuides.Any())
                                        {
                                            var tourGuide = tourGuides.FirstOrDefault();
                                            if (tourGuide != null)
                                            {
                                                tourGuide.Status = 1;
                                                await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                                                await _unitOfWork.SaveAsync();
                                                await transaction.CommitAsync();
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Cannot verify your account");
                                        }
                                    } else
                                    {
                                        await transaction.CommitAsync();
                                    }
                                }
                                else
                                {
                                    throw new Exception("Cannot verify your account");
                                }
                            }
                            else
                            {
                                throw new Exception("Cannot verify your account");
                            }
                        }
                        _memoryCache.Remove(email);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RegisterCustomer(RegisterCustomerRequest registerCustomer)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var account = new Account
                    {
                        Email = registerCustomer.Email,
                        HashedPassword = await HashPassword(registerCustomer.Password),
                        Status = 2,
                        Role = 3
                    };
                    await _unitOfWork.AccountRepository.InsertAsync(account);
                    await _unitOfWork.SaveAsync();

                    var customer = new Customer
                    {
                        AccountId = account.AccountId,
                        FirstName = registerCustomer.FirstName,
                        LastName = registerCustomer.LastName,
                        PhoneNumber = registerCustomer.PhoneNumber,
                        Gender = registerCustomer.Gender,
                        DateOfBirth = registerCustomer.DateOfBirth
                    };

                    await _unitOfWork.CustomerRepository.InsertAsync(customer);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    return true;
                } catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task<bool> RegisterTourGuide(RegisterTourGuideRequest registerTourGuide)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var account = new Account
                    {
                        Email = registerTourGuide.Email,
                        HashedPassword = await HashPassword(registerTourGuide.Password),
                        Status = 2,
                        Role = 2
                    };
                    await _unitOfWork.AccountRepository.InsertAsync(account);
                    await _unitOfWork.SaveAsync();

                    var tourGuide = new TourGuide
                    {
                        AccountId = account.AccountId,
                        FirstName = registerTourGuide.FirstName,
                        LastName = registerTourGuide.LastName,
                        PhoneNumber = registerTourGuide.PhoneNumber,
                        Gender = registerTourGuide.Gender,
                        DateOfBirth = registerTourGuide.DateOfBirth,
                        Address = registerTourGuide.Address,
                        CityId = registerTourGuide.CityId,
                        Status = 0
                    };

                    await _unitOfWork.TourGuideRepository.InsertAsync(tourGuide);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task<bool> CheckExistedEmail(string email)
        {
            try
            {
                var accounts = await _unitOfWork.AccountRepository.FindAsync(a => a.Email == email);
                if (accounts.Any())
                {
                    return true;
                }
                return false;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(string accessToken, string refreshToken)> RefreshingAccessToken(string oldRefreshToken)
        {
            var existRefreshTokens = await _unitOfWork.RefreshTokenRepository.FindAsync(r => r.Token == oldRefreshToken);
            if (existRefreshTokens.Any())
            {
                var existRefreshToken = existRefreshTokens.FirstOrDefault();
                if (existRefreshToken != null)
                {
                    try
                    {
                        var account = await _unitOfWork.AccountRepository.GetByIDAsync(existRefreshToken.AccountId);
                        if (account != null)
                        {
                            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


                            var accessClaims = new List<Claim>
                            {
                            new Claim("Role", account.Role.ToString()),
                            new Claim("Email", account.Email),
                            new Claim("AccountId", account.AccountId.ToString())
                            };

                            var accessExpiration = DateTime.UtcNow.AddHours(1);
                            var accessJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], accessClaims, expires: accessExpiration, signingCredentials: credentials);
                            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(accessJwt);

                            var refreshClaims = new List<Claim>
                            {
                            new Claim("Email", account.Email)
                            };
                            var refreshExpiration = DateTime.UtcNow.AddDays(14);
                            var refreshJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], refreshClaims, expires: refreshExpiration, signingCredentials: credentials);
                            var newRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshJwt);

                            existRefreshToken.Token = newRefreshToken;
                            existRefreshToken.ExpiredDate = refreshExpiration;

                            await _unitOfWork.RefreshTokenRepository.UpdateAsync(existRefreshToken);
                            await _unitOfWork.SaveAsync();

                            return (newAccessToken, newRefreshToken);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            return (null, null);
        }
    }
}
