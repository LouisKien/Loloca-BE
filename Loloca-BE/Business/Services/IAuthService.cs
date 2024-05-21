using Loloca_BE.Business.Models.AccountView;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace Loloca_BE.Business.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateUser(AuthRequest loginInfo);
        Task<bool> AuthenticateUserAdvanced(AuthResponse authResponse);
        Task<string?> HashPassword(string password);
        Task<(string accessToken, string refreshToken)> GenerateTokens(AuthResponse authResponse);
        Task<(string accessToken, string refreshToken)> GenerateTokens(string email, string verificationCode);
        Task<bool> VerifyAccount(string email, string verificationCode);
        Task SendVerificationEmail(string email);
        Task<bool> RegisterCustomer(RegisterCustomerRequest registerCustomer);
        Task<bool> RegisterTourGuide(RegisterTourGuideRequest registerTourGuide);
        Task<bool> CheckExistedEmail(string email);
    }
}
