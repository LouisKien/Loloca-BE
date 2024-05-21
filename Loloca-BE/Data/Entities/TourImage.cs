using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class TourImage
    {
        public int ImageId { get; set; }
        public int TourId { get; set; }
        public string? ImagePath { get; set; }
        public string? Caption { get; set; }
        public DateTime? UploadDate { get; set; }

        public virtual Tour Tour { get; set; } = null!;
    }
}
