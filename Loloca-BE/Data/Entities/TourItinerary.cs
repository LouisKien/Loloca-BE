using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class TourItinerary
    {
        public int ItineraryId { get; set; }
        public int TourId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual Tour Tour { get; set; } = null!;
    }
}
