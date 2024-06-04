namespace Loloca_BE.Business.Models.BookingTourRequestModelView
{
    public class BookingTourRequestView
    {
        public int TourId { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Note { get; set; }
    }
}
