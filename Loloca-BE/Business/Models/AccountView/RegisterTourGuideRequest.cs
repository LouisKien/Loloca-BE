namespace Loloca_BE.Business.Models.AccountView
{
    public class RegisterTourGuideRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int CityId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
