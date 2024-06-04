namespace Loloca_BE.Business.BackgroundServices.Interfaces
{
    public interface IBookingTourGuideRequestBackgroundService
    {
        Task RejectTimeOutBookingTourGuideRequest();
        Task CompletedBookingTourGuideRequest();
    }
}
