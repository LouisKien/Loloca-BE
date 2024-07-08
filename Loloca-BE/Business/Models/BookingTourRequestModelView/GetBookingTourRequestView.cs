namespace Loloca_BE.Business.Models.BookingTourRequestModelView
{
    public class GetBookingTourRequestView
    {
        public int BookingTourRequestId { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime RequestTimeOut { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumOfAdult { get; set; }
        public int NumOfChild { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
    }
}
