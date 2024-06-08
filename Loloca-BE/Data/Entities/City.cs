using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class City
    {
        public City()
        {
            TourGuides = new HashSet<TourGuide>();
            Tours = new HashSet<Tour>();
        }

        public int CityId { get; set; }
        public string Name { get; set; } = null!;
        public string? CityBanner { get; set; }
        public string? CityThumbnail { get; set; }
        public DateTime? CityBannerUploadDate { get; set; }
        public DateTime? CityThumbnailUploadDate { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<TourGuide> TourGuides { get; set; }
        public virtual ICollection<Tour> Tours { get; set; }
    }
}
