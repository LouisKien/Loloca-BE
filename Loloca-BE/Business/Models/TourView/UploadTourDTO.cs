namespace Loloca_BE.Business.Models.TourView
{
    public class UploadTourDTO
    {
        public int CityId { get; set; }
        public int TourGuideId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Activity { get; set; }
        public int? Duration { get; set; }

        public List<string>? ExcludeDetails { get; set; }
        public List<string>? HighlightDetails { get; set; }
        public List<string>? IncludeDetails { get; set; }
        public List<string>? ItineraryNames { get; set; }
        public List<string>? ItineraryDescriptions { get; set; }
        public List<string>? TypeDetails { get; set; }
        public List<int?>? TotalTouristFrom { get; set; }
        public List<int?>? TotalTouristTo { get; set; }
        public List<decimal?>? AdultPrices { get; set; }
        public List<decimal?>? ChildPrices { get; set; }
    }
}
