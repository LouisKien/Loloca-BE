using Loloca_BE.Business.Models.TourGuideView;

namespace Loloca_BE.Business.Services
{
    public interface ITourGuideService
    {
        Task UploadAvatarAsync(IFormFile file, int TourGuideId);
        Task UploadCoverAsync(IFormFile file, int TourGuideId);

        Task UpdateTourGuideInfo(int tourguideId, UpdateProfileTourGuide model);
        Task<bool> ChangeTourGuidePassword(int tourguideId, ChangePasswordTourGuide model);
        Task<GetTourGuideInfo> GetTourGuideInfoAsync(int tourGuideId);
    }
}
