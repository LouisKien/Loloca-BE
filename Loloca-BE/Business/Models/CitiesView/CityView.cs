namespace Loloca_BE.Business.Models.CitiesView
{
    public class CityView
    {
        public int CityId { get; set; }
        public string Name { get; set; } = null!;
        public byte[]? CityBanner { get; set; }
        public byte[]? CityThumbnail { get; set; }
        public DateTime? CityBannerUploadDate { get; set; }
        public DateTime? CityThumbnailUploadDate { get; set; }
        public bool Status { get; set; }
    }
}
