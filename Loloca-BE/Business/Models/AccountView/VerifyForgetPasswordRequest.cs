namespace Loloca_BE.Business.Models.AccountView
{
    public class VerifyForgetPasswordRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
