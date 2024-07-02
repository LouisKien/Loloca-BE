namespace Loloca_BE.Business.Models.FeedbackView
{
    public class GetFeedbackForTourGuideView
    {
        public int FeedbackId { get; set; } // Thêm thuộc tính FeedbackId để xác định phản hồi cụ thể
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int TourGuideId { get; set; }
        public int? BookingTourRequestsId { get; set; }
        public int? BookingTourGuideRequestId { get; set; }
        public int NumOfStars { get; set; }
        public string? Content { get; set; }
        public DateTime? TimeFeedback { get; set; }
        public bool Status { get; set; }

        public List<FeedbackImageView>? feedBackImgViewListForTourGuide { get; set; }


    }
}
