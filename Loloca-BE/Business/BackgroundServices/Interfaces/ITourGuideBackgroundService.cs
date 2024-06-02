namespace Loloca_BE.Business.BackgroundServices.Interfaces
{
    public interface ITourGuideBackgroundService
    {
        Task RefreshTourGuideCache();
        Task RefreshTourGuideInCityCache();
    }
}
