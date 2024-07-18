namespace Loloca_BE.Business.Models.TourGuideView
{
    public class TourGuideWithCityNameDTO
    {
        public int TourGuideId { get; set; }
        public int AccountId { get; set; }
        public int CityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ZaloLink { get; set; }
        public string FacebookLink { get; set; }
        public string InstagramLink { get; set; }
        public decimal? PricePerDay { get; set; }
        public int Status { get; set; }
        public byte[]? Avatar { get; set; }
        public DateTime? AvatarUploadedTime { get; set; }
        public string CityName { get; set; }
    }
}
