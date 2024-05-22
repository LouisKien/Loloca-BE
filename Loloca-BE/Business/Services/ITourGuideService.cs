namespace Loloca_BE.Business.Services
{
    public interface ITourGuideService
    {
        Task UploadAvatarAsync(IFormFile file, int TourGuideId);
        Task UploadCoverAsync(IFormFile file, int TourGuideId);
    }
}
