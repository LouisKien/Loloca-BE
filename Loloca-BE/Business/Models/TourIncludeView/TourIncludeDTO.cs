namespace Loloca_BE.Business.Models.TourIncludeView
{
    public class TourIncludeDTO
    {
        public int IncludeId { get; set; }
        public int TourId { get; set; }
        public string IncludeDetail { get; set; } = null!;
    }
}
