namespace Loloca_BE.Business.BackgroundServices.Interfaces
{
    public interface ITourBackgroundService
    {
        Task RefreshTourCache();

        Task RefreshTourInCityCache();
    }
}
