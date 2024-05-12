using Loloca_BE.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace Loloca_BE.Business.Services
{
    public interface IAccountsService
    {
        Task<AccountsView?> AuthenticateUser(AccountsView loginInfo);
        Task<string?> HashPassword(string password);
        Task<(string accessToken, string refreshToken)> GenerateTokens(AccountsView account);
    }
}
