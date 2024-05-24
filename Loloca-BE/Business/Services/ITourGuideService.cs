using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services
{
    public interface ITourGuideService
    {
        Task UploadAvatarAsync(IFormFile file, int TourGuideId);
        Task UploadCoverAsync(IFormFile file, int TourGuideId);

        Task UpdateTourGuideInfo(int tourguideId, UpdateProfileTourGuide model);
        Task<bool> ChangeTourGuidePassword(int tourguideId, ChangePasswordTourGuide model);
        Task<GetTourGuideInfo> GetTourGuideInfoAsync(int tourGuideId);
        Task<List<GetTourGuide>> GetRandomTourGuidesAsync(string sessionId, int page, int pageSize, int? lastFetchId);
        Task<int?> GetLastTourGuideAddedIdAsync();
        Task<List<GetTourGuide>> GetRandomTourGuidesInCityAsync(string sessionId, int CityId, int page, int pageSize, int? lastFetchId);
        Task<int?> GetLastTourGuideAddedIdInCityAsync(int CityId);
    }
}
