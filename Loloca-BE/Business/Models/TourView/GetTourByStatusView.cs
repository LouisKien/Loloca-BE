namespace Loloca_BE.Business.Models.TourView
{
    public class GetTourByStatusView
    {
        public int TourId { get; set; }
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public int TourGuideId { get; set; }
        public string TourGuideName { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public int Status { get; set; }
        public decimal? Price { get; set; }

        public List<TourImageView>? tourImgViewList { get; set; }
    }
}
