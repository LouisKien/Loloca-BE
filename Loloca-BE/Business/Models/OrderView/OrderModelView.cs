namespace Loloca_BE.Business.Models.OrderView
{
    public class OrderModelView
    {
        public int CustomerId { get; set; }
        public int? BookingTourRequestsId { get; set; }
        public int? BookingTourGuideRequestId { get; set; }
        public string OrderCode { get; set; } = null!;
        public double OrderPrice { get; set; }
        public string? PaymentProvider { get; set; }
        public string? TransactionCode { get; set; }
        public int Status { get; set; }
        
    }
}
