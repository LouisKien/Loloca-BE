using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class TourPrice
    {
        public int TourPriceId { get; set; }
        public int TourId { get; set; }
        public int TotalTouristFrom { get; set; }
        public int TotalTouristTo { get; set; }
        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }

        public virtual Tour Tour { get; set; } = null!;
    }
}
