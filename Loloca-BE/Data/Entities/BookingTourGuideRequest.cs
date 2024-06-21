using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class BookingTourGuideRequest
    {
        public BookingTourGuideRequest()
        {
            Feedbacks = new HashSet<Feedback>();
            Orders = new HashSet<Order>();
        }

        public int BookingTourGuideRequestId { get; set; }
        public int TourGuideId { get; set; }
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

        public virtual Customer Customer { get; set; } = null!;
        public virtual TourGuide TourGuide { get; set; } = null!;
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
