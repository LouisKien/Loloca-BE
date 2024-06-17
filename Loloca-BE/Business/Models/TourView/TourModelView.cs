namespace Loloca_BE.Business.Models.TourView
{
    public class TourModelView
    {
        public int CityId { get; set; }
        public int TourGuideId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Activity { get; set; }
        public int? Duration { get; set; }
    }
}
