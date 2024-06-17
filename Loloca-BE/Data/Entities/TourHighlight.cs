using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class TourHighlight
    {
        public int HighlightId { get; set; }
        public int TourId { get; set; }
        public string HighlightDetail { get; set; } = null!;

        public virtual Tour Tour { get; set; } = null!;
    }
}
