namespace Loloca_BE.Business.Models.TourGuideView
{
    public class GetTourGuide
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Description { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public decimal? PricePerDay { get; set; }
        public byte[]? Avatar { get; set; }
        public DateTime? AvatarUploadedTime { get; set; }
    }
}
