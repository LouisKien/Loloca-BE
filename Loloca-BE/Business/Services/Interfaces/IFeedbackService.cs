using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.OrderView;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<FeebackView?> GetFeedbackByIdAsync(int id);
        Task<IEnumerable<GetFeedBackForCustomerView>> GetFeedbackByCustomerIdAsync(int customerId);
        Task<IEnumerable<GetFeedbackForTourGuideView>> GetFeedbackByTourGuideIdAsync(int tourGuideId);
        Task<IEnumerable<FeebackView>> GetAllFeedbacksAsync();

        Task UploadFeedbackAsync(FeedbackModelView feedbackModel, List<IFormFile> images);
        Task<bool> UpdateStatusAsync(UpdateFeedbackStatusView updateFeedbackStatusView);

        Task<(int count, float average)> GetFeedbackStatsAsync(int id, bool isTour);

        Task<IEnumerable<FeebackView>> GetFeedbacksByTourIdAsync(int tourId);

        Task<IEnumerable<FeebackView>> GetFeedbacksByCityIdAsync(int cityId);
    }
}
