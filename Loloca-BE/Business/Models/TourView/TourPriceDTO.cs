namespace Loloca_BE.Business.Models.TourView
{
    public class TourPriceDTO
    {
        public int? TotalTouristFrom { get; set; }
        public int? TotalTouristTo { get; set; }
        public decimal? AdultPrice { get; set; }
        public decimal? ChildPrice { get; set; }
    }
}
