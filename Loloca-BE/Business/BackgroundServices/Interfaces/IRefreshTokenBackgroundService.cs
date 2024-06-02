namespace Loloca_BE.Business.BackgroundServices.Interfaces
{
    public interface IRefreshTokenBackgroundService
    {
        Task RemoveExpiredRefreshToken();
    }
}
