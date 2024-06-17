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

        public List<TourExcludeDTO>? tourExcludeDTOs { get; set; }
        public List<TourHighlightDTO>? tourHighlightDTOs { get; set; }
        public List<TourIncludeDTO>? tourIncludeDTOs { get; set; }
        public List<TourItineraryDTO>? tourItineraryDTOs { get; set; }
        public List<TourTypeDTO>? tourTypeDTOs { get; set; }
        public List<TourPriceDTO>? tourPriceDTOs { get; set; }
    }
}
