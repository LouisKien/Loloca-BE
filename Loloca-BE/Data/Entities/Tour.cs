using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class Tour
    {
        public Tour()
        {
            BookingTourRequests = new HashSet<BookingTourRequest>();
            TourExcludes = new HashSet<TourExclude>();
            TourHighlights = new HashSet<TourHighlight>();
            TourImages = new HashSet<TourImage>();
            TourIncludes = new HashSet<TourInclude>();
            TourItineraries = new HashSet<TourItinerary>();
            TourTypes = new HashSet<TourType>();
        }

        public int TourId { get; set; }
        public int CityId { get; set; }
        public int TourGuideId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public decimal? Price { get; set; }
        public int Status { get; set; }

        public virtual City City { get; set; } = null!;
        public virtual TourGuide TourGuide { get; set; } = null!;
        public virtual ICollection<BookingTourRequest> BookingTourRequests { get; set; }
        public virtual ICollection<TourExclude> TourExcludes { get; set; }
        public virtual ICollection<TourHighlight> TourHighlights { get; set; }
        public virtual ICollection<TourImage> TourImages { get; set; }
        public virtual ICollection<TourInclude> TourIncludes { get; set; }
        public virtual ICollection<TourItinerary> TourItineraries { get; set; }
        public virtual ICollection<TourType> TourTypes { get; set; }
    }
}
