namespace Loloca_BE.Business.BackgroundServices.Interfaces
{
    public interface IPaymentRequestBackgroundService
    {
        Task RejectExpiredPaymentRequest();
    }
}
