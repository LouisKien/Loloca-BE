using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ITourService
    {
        Task UploadTourAsync(UploadTourDTO tourModel, List<IFormFile> images);
        Task UpdateTourAsync(UpdateTourView updateTourView);

        Task UpdateTourStatusAsync(UpdateTourStatusView updateTourStatusView);

        Task DeleteTourAsync(int tourId);
        Task<List<AllToursView>> GetRandomToursAsync(string sessionId, int page, int pageSize);
        Task<List<AllToursView>> GetRandomToursInCityAsync(string sessionId, int CityId, int page, int pageSize);
        Task<List<AllToursView>> GetToursByTourGuideAsync(int TourGuideId, int page, int pageSize);
        Task<GetTourByIdView?> GetTourByIdAsync(int tourId);
        Task<int> GetTotalPage(int pageSize, int? cityId, string sessionId);
        Task<int> GetTotalPageByTourGuide(int TourGuideId, int pageSize);
        Task<List<GetTourByStatusView>> GetToursByStatusAsync(int status);
    }
}
