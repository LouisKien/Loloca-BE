namespace Loloca_BE.Business.Models.OrderView
{
    public class OrderForBookingTourGuideView
    {
        public int CustomerId { get; set; }
        public int? BookingTourGuideRequestId { get; set; }
        public string OrderCode { get; set; } = null!;
        public double OrderPrice { get; set; }
        public string? PaymentProvider { get; set; }
        public string? TransactionCode { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }

    }
}
