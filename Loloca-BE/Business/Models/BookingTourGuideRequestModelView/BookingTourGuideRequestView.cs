namespace Loloca_BE.Business.Models.BookingTourGuideRequestModelView
{
    public class BookingTourGuideRequestView
    {
        public int TourGuideId { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumOfAdult { get; set; }
        public int NumOfChild { get; set; }
        public string? Note { get; set; }
    }
}
