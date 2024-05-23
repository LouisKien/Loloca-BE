namespace Loloca_BE.Business.Models.AccountView
{
    public class ChangeNewPasswordRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
    }
}
