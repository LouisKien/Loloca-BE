using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services
{
    public interface ITourService
    {
        Task UploadTourImageAsync(TourModelView tourModel, List<IFormFile> images);
        Task UpdateTourAsync(int tourId, TourInfoView tourModel);

        Task UpdateTourStatusAsync(int tourId, TourStatusView tourModel);

        Task DeleteTourAsync(int tourId);
        Task<List<AllToursView>> GetRandomToursAsync(string sessionId, int page, int pageSize, int? lastFetchId);
        Task<int> GetTotalPage(int pageSize);
        Task<int?> GetLastTourAddedIdAsync();
    }
}
