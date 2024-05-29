using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.TourView;

namespace Loloca_BE.Business.Services
{
    public interface ITourService
    {
        Task UploadTourImageAsync(TourModelView tourModel, List<IFormFile> images);
        Task UpdateTourAsync(int tourId, TourInfoView tourModel);

        Task UpdateTourStatusAsync(int tourId, TourStatusView tourModel);

    }
}
