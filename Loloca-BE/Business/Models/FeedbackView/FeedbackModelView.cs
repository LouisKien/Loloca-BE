
namespace Loloca_BE.Business.Models.FeedbackView
{
    public class FeedbackModelView
    {
        public int CustomerId { get; set; }
        public int TourGuideId { get; set; }
        public int NumOfStars { get; set; }
        public string? Content { get; set; }
        public DateTime? TimeFeedback { get; set; }
        public bool Status { get; set; }
    }
}
