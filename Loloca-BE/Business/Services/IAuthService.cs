using Loloca_BE.Business.Models.AccountsView;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace Loloca_BE.Business.Services
{
    public interface IAuthService
    {
        Task<(string? email, string? code)> AuthenticateUser(AccountsView loginInfo);
        Task<string?> HashPassword(string password);
        Task<(string accessToken, string refreshToken)> GenerateTokens(string email, string verificationCode);
    }
}
