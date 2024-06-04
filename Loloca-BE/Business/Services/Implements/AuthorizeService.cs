using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.Services.Implements
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorizeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByAccountId(int userAccountId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var account = (await _unitOfWork.AccountRepository.GetByIDAsync(userAccountId));
                if(account != null)
                {
                    if (account.AccountId == accountId)
                    {
                        isUser = true;
                    }
                }
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.Role == 1)
                    {
                        isAdmin = true;
                    }
                }
                return (isUser, isAdmin);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByBookingTourGuideRequestId(int bookingTourGuideRequestId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.Role == 2)
                    {
                        var tourGuide = (await _unitOfWork.TourGuideRepository.FindAsync(t => t.AccountId == accountId)).FirstOrDefault();
                        if (tourGuide != null)
                        {
                            var btgr = await _unitOfWork.BookingTourGuideRepository.GetByIDAsync(bookingTourGuideRequestId);
                            if (btgr != null)
                            {
                                if (btgr.TourGuideId == tourGuide.TourGuideId)
                                {
                                    isUser = true;
                                }
                            }
                        }
                    }
                    else if (accountJwt.Role == 3)
                    {
                        var customer = (await _unitOfWork.CustomerRepository.FindAsync(c => c.AccountId == accountId)).FirstOrDefault();
                        if (customer != null)
                        {
                            var btgr = await _unitOfWork.BookingTourGuideRepository.GetByIDAsync(bookingTourGuideRequestId);
                            if (btgr != null)
                            {
                                if (btgr.CustomerId == customer.CustomerId)
                                {
                                    isUser = true;
                                }
                            }
                        }
                    }
                    if(accountJwt.Role == 1)
                    {
                        isAdmin = true;
                    }
                }
                return (isUser, isAdmin);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByBookingTourRequestId(int bookingTourRequestId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.Role == 2)
                    {
                        var tourGuide = (await _unitOfWork.TourGuideRepository.FindAsync(t => t.AccountId == accountId)).FirstOrDefault();
                        if (tourGuide != null)
                        {
                            var btr = await _unitOfWork.BookingTourRequestRepository.GetByIDAsync(bookingTourRequestId);
                            if (btr != null)
                            {
                                var tour = await _unitOfWork.TourGuideRepository.GetByIDAsync(bookingTourRequestId);
                                if (tour != null)
                                {
                                    if(tour.TourGuideId == tourGuide.TourGuideId)
                                    {
                                        isUser = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (accountJwt.Role == 3)
                    {
                        var customer = (await _unitOfWork.CustomerRepository.FindAsync(c => c.AccountId == accountId)).FirstOrDefault();
                        if (customer != null)
                        {
                            var btr = await _unitOfWork.BookingTourRequestRepository.GetByIDAsync(bookingTourRequestId);
                            if (btr != null)
                            {
                                if (btr.CustomerId == customer.CustomerId)
                                {
                                    isUser = true;
                                }
                            }
                        }
                    }
                    if (accountJwt.Role == 1)
                    {
                        isAdmin = true;
                    }
                }
                return (isUser, isAdmin);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByCustomerId(int customerId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(customerId);
                if (customer != null)
                {
                    if(customer.AccountId == accountId)
                    {
                        isUser = true;
                    }
                }
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.Role == 1)
                    {
                        isAdmin = true;
                    }
                }
                return (isUser, isAdmin);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByFeedbackId(int feedbackId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.Role == 2)
                    {
                        var tourGuide = (await _unitOfWork.TourGuideRepository.FindAsync(t => t.AccountId == accountId)).FirstOrDefault();
                        if (tourGuide != null)
                        {
                            var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId);
                            if (feedback != null)
                            {
                                if(feedback.TourGuideId == tourGuide.TourGuideId)
                                {
                                    isUser = true;
                                }
                            }
                        }
                    } else if (accountJwt.Role == 3)
                    {
                        var customer = (await _unitOfWork.CustomerRepository.FindAsync(c => c.AccountId == accountId)).FirstOrDefault();
                        if (customer != null)
                        {
                            var feedback = await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId);
                            if (feedback != null)
                            {
                                if (feedback.CustomerId == customer.CustomerId)
                                {
                                    isUser = true;
                                }
                            }
                        }
                    }
                    if (accountJwt.Role == 1)
                    {
                        isAdmin = true;
                    }
                }
                return (isUser, isAdmin);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByNotificationId(int notificationId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var notification = await _unitOfWork.NotificationRepository.GetByIDAsync(notificationId);
                if(notification != null)
                {
                    switch (notification.UserType)
                    {
                        case "1":
                            isAdmin = true;
                            break;
                        case "2":
                            var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == accountId)).FirstOrDefault();
                            if(tourGuide != null)
                            {
                                isUser = true;
                            }
                            break;
                        case "3":
                            var customer = (await _unitOfWork.CustomerRepository.GetAsync(c => c.AccountId == accountId)).FirstOrDefault();
                            if(customer != null)
                            {
                                isUser = true;
                            }
                            break;
                    }
                }
                return (isUser, isAdmin);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByOrderId(int orderId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var order = await _unitOfWork.OrderRepository.GetByIDAsync(orderId);
                if (order != null)
                {
                    var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(order.CustomerId);
                    if(customer != null)
                    {
                        if(customer.AccountId == accountId)
                        {
                            isUser = true;
                        }
                    }
                }
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.Role == 1)
                    {
                        isAdmin = true;
                    }
                }
                return (isUser, isAdmin);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByPaymentRequestId(int paymentRequestId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var pr = await _unitOfWork.PaymentRequestRepository.GetByIDAsync(paymentRequestId);
                if (pr != null)
                {
                    if(pr.AccountId == accountId)
                    {
                        isUser = true;
                    }
                }
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.Role == 1)
                    {
                        isAdmin = true;
                    }
                }
                return (isUser, isAdmin);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByTourGuideId(int tourGuideId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(tourGuideId);
                if (tourGuide != null)
                {
                    if(tourGuide.AccountId == accountId)
                    {
                        isUser = true;
                    }
                }
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.Role == 1)
                    {
                        isAdmin = true;
                    }
                }
                return (isUser, isAdmin);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isUser, bool isAdmin)> CheckAuthorizeByTourId(int tourId, int accountId)
        {
            try
            {
                bool isAdmin = false;
                bool isUser = false;
                var tour = await _unitOfWork.TourRepository.GetByIDAsync(tourId);
                if(tour != null)
                {
                    var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(tour.TourGuideId);
                    if (tourGuide != null)
                    {
                        if(tourGuide.AccountId == accountId)
                        {
                            isUser = true;
                        }
                    }
                }
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.Role == 1)
                    {
                        isAdmin = true;
                    }
                }
                return (isUser, isAdmin);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
