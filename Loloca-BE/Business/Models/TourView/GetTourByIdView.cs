using Loloca_BE.Business.Models.FeedbackView;

namespace Loloca_BE.Business.Models.TourView
{
    public class GetTourByIdView
    {
        public int TourId { get; set; }
        public int CityId { get; set; }
        public int TourGuideId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Activity { get; set; }
        public int? Duration { get; set; }
        public int Status { get; set; }

        public List<TourImageView> tourImgViewList { get; set; } = new List<TourImageView>();
        public List<TourExcludeDTO> tourExcludeDTOs { get; set; } = new List<TourExcludeDTO>();
        public List<TourHighlightDTO> tourHighlightDTOs { get; set; } = new List<TourHighlightDTO> { };
        public List<TourIncludeDTO> tourIncludeDTOs { get; set; } = new List<TourIncludeDTO> { };
        public List<TourItineraryDTO> tourItineraryDTOs { get; set; } = new List<TourItineraryDTO> { };
        public List<TourTypeDTO> tourTypeDTOs { get; set; } = new List<TourTypeDTO> { };
        public List<TourPriceDTO> tourPriceDTOs { get; set; } = new List<TourPriceDTO> { };
    }
}
