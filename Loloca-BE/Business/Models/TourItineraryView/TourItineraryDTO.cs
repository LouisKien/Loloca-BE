namespace Loloca_BE.Business.Models.TourItineraryView
{
    public class TourItineraryDTO
    {
        public int ItineraryId { get; set; }
        public int TourId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
