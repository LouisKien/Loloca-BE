namespace Loloca_BE.Business.Models.FeedbackView
{
    public class GetFeedBackForCustomerView
    {
        public int FeedbackId { get; set; }
        public int CustomerId { get; set; }
        public int TourGuideId { get; set; }
        public int NumOfStars { get; set; }
        public string? Content { get; set; }
        public DateTime? TimeFeedback { get; set; }
        public bool Status { get; set; }

        public List<FeedbackImageView>? feedBackImgViewListForCus { get; set; }

    }
}
