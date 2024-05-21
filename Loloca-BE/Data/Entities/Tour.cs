using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class Tour
    {
        public Tour()
        {
            BookingTourRequests = new HashSet<BookingTourRequest>();
            TourImages = new HashSet<TourImage>();
        }

        public int TourId { get; set; }
        public int CityId { get; set; }
        public int TourGuideId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public int Status { get; set; }

        public virtual City City { get; set; } = null!;
        public virtual TourGuide TourGuide { get; set; } = null!;
        public virtual ICollection<BookingTourRequest> BookingTourRequests { get; set; }
        public virtual ICollection<TourImage> TourImages { get; set; }
    }
}
