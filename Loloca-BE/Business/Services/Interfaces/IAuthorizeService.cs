namespace Loloca_BE.Business.Services.Interfaces
{
    public interface IAuthorizeService
    {
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByAccountId(int userAccountId, int accountId);
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByCustomerId(int customerId, int accountId);
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByTourGuideId(int tourGuideId, int accountId);
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByPaymentRequestId(int paymentRequestId, int accountId);
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByTourId(int tourId, int accountId);
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByFeedbackId(int feedbackId, int accountId);
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByBookingTourGuideRequestId(int bookingTourGuideRequestId, int accountId);
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByBookingTourRequestId(int bookingTourRequestId, int accountId);
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByOrderId(int orderId, int accountId);
        Task<(bool isUser, bool isAdmin)> CheckAuthorizeByNotificationId(int notificationId, int accountId);
    }
}
