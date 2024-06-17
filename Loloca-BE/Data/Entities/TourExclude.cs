using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class TourExclude
    {
        public int ExcludeId { get; set; }
        public int TourId { get; set; }
        public string ExcludeDetail { get; set; } = null!;

        public virtual Tour Tour { get; set; } = null!;
    }
}
