using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.Net;
using System.Net.Mail;

namespace Loloca_BE.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _credentialsPath;
        private UserCredential _credential;

        public EmailService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _credentialsPath = Path.Combine(env.ContentRootPath, "credentials.json");
        }

        private async Task<UserCredential> GetCredentialAsync()
        {
            if (_credential != null && !_credential.Token.IsExpired(_credential.Flow.Clock))
            {
                return _credential;
            }

            using (var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
            {
                _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { GmailService.Scope.GmailSend },
                    "user", CancellationToken.None,
                    new FileDataStore("Token.json", true));
            }

            return _credential;
        }

        public async Task SendVerificationEmailAsync(string email, string verificationCode)
        {
            var credential = await GetCredentialAsync();

            var service = new GmailService(new BaseClientService.Initializer()
            {
                ApplicationName = "LOLOCA",
                HttpClientInitializer = credential
            });

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("LOLOCA", "louisnamu2002@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "LOLOCA: Email Verification Code";
            message.Body = new TextPart("plain")
            {
                Text = $"Your verification code is: {verificationCode}.\nPlease enter this code to verify, it will be expired after 1 hour.\n\nBest regards\nLOLOCA Team"
            };

            var gmailMessage = new Message
            {
                Raw = Base64UrlEncoder.Encode(message.ToString())
            };

            await service.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
        }
    }
}
