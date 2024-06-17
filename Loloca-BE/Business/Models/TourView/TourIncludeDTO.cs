namespace Loloca_BE.Business.Models.TourView
{
    public class TourIncludeDTO
    {
        public int IncludeId { get; set; }
        public int TourId { get; set; }
        public string IncludeDetail { get; set; } = null!;
    }
}
