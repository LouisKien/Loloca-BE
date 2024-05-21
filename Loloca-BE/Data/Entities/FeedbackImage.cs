using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class FeedbackImage
    {
        public int FeedbackImageId { get; set; }
        public int FeedbackId { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? UploadDate { get; set; }

        public virtual Feedback Feedback { get; set; } = null!;
    }
}
