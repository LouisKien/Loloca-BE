using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

namespace Loloca_BE.Business.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string verificationCode);
    }
}
