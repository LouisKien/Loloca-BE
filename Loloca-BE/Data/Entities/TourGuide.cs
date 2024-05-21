using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class TourGuide
    {
        public TourGuide()
        {
            BookingTourGuideRequests = new HashSet<BookingTourGuideRequest>();
            Feedbacks = new HashSet<Feedback>();
            Tours = new HashSet<Tour>();
        }

        public int TourGuideId { get; set; }
        public int AccountId { get; set; }
        public int CityId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ZaloLink { get; set; }
        public string? FacebookLink { get; set; }
        public string? InstagramLink { get; set; }
        public decimal? PricePerDay { get; set; }
        public int Status { get; set; }
        public string? AvatarPath { get; set; }
        public DateTime? AvatarUploadDate { get; set; }
        public string? CoverPath { get; set; }
        public DateTime? CoverUploadDate { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual City City { get; set; } = null!;
        public virtual ICollection<BookingTourGuideRequest> BookingTourGuideRequests { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
    }
}
