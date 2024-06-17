namespace Loloca_BE.Business.Models.TourTypeView
{
    public class TourTypeDTO
    {
        public int TypeId { get; set; }
        public int TourId { get; set; }
        public string TypeDetail { get; set; } = null!;
    }
}
