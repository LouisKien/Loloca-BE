using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.OrderView;

namespace Loloca_BE.Business.Services
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackModelView>> GetAllFeedbacksAsync();
        Task<FeedbackModelView?> GetFeedbackByIdAsync(int id);

        Task UploadFeedbackAsync(FeedbackModelView feedbackModel, List<IFormFile> images);

    }
}
