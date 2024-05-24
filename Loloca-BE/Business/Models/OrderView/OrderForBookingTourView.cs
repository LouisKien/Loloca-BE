namespace Loloca_BE.Business.Models.OrderView
{
    public class OrderForBookingTourView
    {
        public int CustomerId { get; set; }
        public int? BookingTourRequestsId { get; set; }
        public string OrderCode { get; set; } = null!;
        public double OrderPrice { get; set; }
        public string? PaymentProvider { get; set; }
        public string? TransactionCode { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }


    }
}
