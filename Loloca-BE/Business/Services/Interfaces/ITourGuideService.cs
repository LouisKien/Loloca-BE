using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface ITourGuideService
    {
        Task UploadAvatarAsync(IFormFile file, int TourGuideId);
        Task UploadCoverAsync(IFormFile file, int TourGuideId);

        Task UpdateTourGuideInfo(int tourguideId, UpdateProfileTourGuide model);
        Task<bool> ChangeTourGuidePassword(int tourguideId, ChangePasswordTourGuide model);
        Task<GetTourGuideInfo> GetTourGuideInfoAsync(int tourGuideId);
        Task<GetTourGuidePrivateInfo> GetPrivateTourGuideInfoAsync(int tourGuideId);
        Task<List<GetTourGuide>> GetRandomTourGuidesAsync(string sessionId, int page, int pageSize);
        Task<List<GetTourGuide>> GetRandomTourGuidesInCityAsync(string sessionId, int CityId, int page, int pageSize);
        Task<int> GetTotalPage(int pageSize, int? cityId, string sessionId);
        Task<int> GetTotalPage(int pageSize);
        Task<List<GetTourGuideInfo>> GetTourGuidesAsync(int page, int pageSize);
        Task<bool> AcceptRequestBookingTourGuideRequest(int bookingRequestId);
        Task<bool> RejectRequestBookingTourGuideRequest(int bookingRequestId);

        Task<bool> AcceptRequestBookingTourRequest(int bookingTourRequestId);
        Task<bool> RejectRequestBookingTourRequest(int bookingTourRequestId);
    }
}
