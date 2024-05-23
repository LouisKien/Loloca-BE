namespace Loloca_BE.Business.Models.CustomerView
{
    public class UpdateProfile
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AddressCustomer { get; set; }
    }
}
