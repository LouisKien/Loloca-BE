namespace Loloca_BE.Business.Models.AccountView
{
    public class AuthResponse
    {
        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public int Role { get; set; }
        public int Status { get; set; }
    }
}
