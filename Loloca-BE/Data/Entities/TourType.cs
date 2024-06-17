using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class TourType
    {
        public int TypeId { get; set; }
        public int TourId { get; set; }
        public string TypeDetail { get; set; } = null!;

        public virtual Tour Tour { get; set; } = null!;
    }
}
