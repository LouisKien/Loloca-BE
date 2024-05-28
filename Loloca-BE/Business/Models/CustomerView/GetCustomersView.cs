namespace Loloca_BE.Business.Models.CustomerView
{
    public class GetCustomersView
    {
        public int? CustomerId { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public int? Gender { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AddressCustomer { get; set; }
        public byte[]? Avatar { get; set; }
        public DateTime? AvatarUploadTime { get; set; }
        public int? AccountStatus { get; set; }
    }
}
