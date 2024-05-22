using System;
using System.Collections.Generic;

namespace Loloca_BE.Data.Entities
{
    public partial class Feedback
    {
        public Feedback()
        {
            FeedbackImages = new HashSet<FeedbackImage>();
        }

        public int FeedbackId { get; set; }
        public int CustomerId { get; set; }
        public int TourGuideId { get; set; }
        public int NumOfStars { get; set; }
        public string? Content { get; set; }
        public DateTime? TimeFeedback { get; set; }
        public bool Status { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual TourGuide TourGuide { get; set; } = null!;
        public virtual ICollection<FeedbackImage> FeedbackImages { get; set; }
    }
}
