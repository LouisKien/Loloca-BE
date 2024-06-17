﻿using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class TourInclude
    {
        public int IncludeId { get; set; }
        public int TourId { get; set; }
        public string IncludeDetail { get; set; } = null!;

        public virtual Tour Tour { get; set; } = null!;
    }
}
