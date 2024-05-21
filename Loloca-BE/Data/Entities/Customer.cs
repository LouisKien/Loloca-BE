﻿using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class Customer
    {
        public Customer()
        {
            BookingTourGuideRequests = new HashSet<BookingTourGuideRequest>();
            BookingTourRequests = new HashSet<BookingTourRequest>();
            Feedbacks = new HashSet<Feedback>();
            Orders = new HashSet<Order>();
        }

        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? AvatarPath { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<BookingTourGuideRequest> BookingTourGuideRequests { get; set; }
        public virtual ICollection<BookingTourRequest> BookingTourRequests { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
