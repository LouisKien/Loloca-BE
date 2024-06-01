using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
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

        public async Task<bool> CheckAuthorizeByAccountId(int userAccountId, int accountId)
        {
            try
            {
                var account = (await _unitOfWork.AccountRepository.GetByIDAsync(userAccountId));
                if(account != null)
                {
                    if (account.AccountId == accountId)
                    {
                        return true;
                    }
                }
                return false;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckAuthorizeByBookingTourGuideRequestId(int bookingTourGuideRequestId, int accountId)
        {
            try
            {
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
                                    return true;
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
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckAuthorizeByBookingTourRequestId(int bookingTourRequestId, int accountId)
        {
            try
            {
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
                                        return true;
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
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckAuthorizeByCustomerId(int customerId, int accountId)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(customerId);
                if (customer != null)
                {
                    if(customer.AccountId == accountId)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckAuthorizeByFeedbackId(int feedbackId, int accountId)
        {
            try
            {
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
                                    return true;
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
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckAuthorizeByOrderId(int orderId, int accountId)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIDAsync(orderId);
                if (order != null)
                {
                    var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(order.CustomerId);
                    if(customer != null)
                    {
                        if(customer.AccountId == accountId)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckAuthorizeByPaymentRequestId(int paymentRequestId, int accountId)
        {
            try
            {
                var pr = await _unitOfWork.PaymentRequestRepository.GetByIDAsync(paymentRequestId);
                if (pr != null)
                {
                    if(pr.AccountId == accountId)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckAuthorizeByTourGuideId(int tourGuideId, int accountId)
        {
            try
            {
                var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(tourGuideId);
                if (tourGuide != null)
                {
                    if(tourGuide.AccountId == accountId)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckAuthorizeByTourId(int tourId, int accountId)
        {
            try
            {
                var tour = await _unitOfWork.TourRepository.GetByIDAsync(tourId);
                if(tour != null)
                {
                    var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(tour.TourGuideId);
                    if (tourGuide != null)
                    {
                        if(tourGuide.AccountId == accountId)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
