namespace Loloca_BE.Business.BackgroundServices.Interfaces
{
    public interface IOrderBackgroundService
    {
        Task RejectExpiredOrder();
    }
}
