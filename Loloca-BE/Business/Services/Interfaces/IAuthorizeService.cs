namespace Loloca_BE.Business.Services.Interfaces
{
    public interface IAuthorizeService
    {
        Task<bool> CheckAuthorizeByAccountId(int userAccountId, int accountId);
        Task<bool> CheckAuthorizeByCustomerId(int customerId, int accountId);
        Task<bool> CheckAuthorizeByTourGuideId(int tourGuideId, int accountId);
        Task<bool> CheckAuthorizeByPaymentRequestId(int paymentRequestId, int accountId);
        Task<bool> CheckAuthorizeByTourId(int tourId, int accountId);
        Task<bool> CheckAuthorizeByFeedbackId(int feedbackId, int accountId);
        Task<bool> CheckAuthorizeByBookingTourGuideRequestId(int bookingTourGuideRequestId, int accountId);
        Task<bool> CheckAuthorizeByBookingTourRequestId(int bookingTourRequestId, int accountId);
        Task<bool> CheckAuthorizeByOrderId(int orderId, int accountId);
    }
}
