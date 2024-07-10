namespace Loloca_BE.Business.Models.TourGuideView
{
    public class GetTourGuideInfo
    {
        public int? AccountStatus {  get; set; }
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string? Description { get; set; }
        public string? ZaloLink { get; set; }
        public string? FacebookLink { get; set; }
        public string? InstagramLink { get; set; }
        public decimal? PricePerDay { get; set; }
        public byte[]? Avatar { get; set; }
        public DateTime? AvatarUploadedTime { get; set; }
        public byte[]? Cover { get; set; }
        public DateTime? CoverUploadedTime { get; set; }
    }
}
