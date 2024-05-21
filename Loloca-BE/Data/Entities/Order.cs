using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int? BookingTourRequestsId { get; set; }
        public int? BookingTourGuideRequestId { get; set; }
        public string OrderCode { get; set; } = null!;
        public double OrderPrice { get; set; }
        public string? PaymentProvider { get; set; }
        public string? TransactionCode { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual BookingTourGuideRequest? BookingTourGuideRequest { get; set; }
        public virtual BookingTourRequest? BookingTourRequests { get; set; }
        public virtual Customer Customer { get; set; } = null!;
    }
}
