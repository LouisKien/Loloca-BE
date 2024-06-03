namespace Loloca_BE.Business.BackgroundServices.Interfaces
{
    public interface IAccountBackgroundService
    {
        Task LockSpammedCustomerAccount();
        Task LockSpammedTourGuideAccount();
        Task UnlockSpammedCustomerAccount();
        Task UnlockSpammedTourGuideAccount();
    }
}
