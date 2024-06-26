﻿namespace Loloca_BE.Business.Models.BookingTourRequestModelView
{
    public class GetBookingTourRequestView
    {
        public int BookingTourRequestId { get; set; }
        public int TourId { get; set; }
        public int CustomerId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime RequestTimeOut { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
    }
}
