using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.BackgroundServices.Implements
{
    public class BookingTourRequestBackgroundService : IBookingTourRequestBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingTourRequestBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CompletedBookingTourRequest()
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var btgr = await _unitOfWork.BookingTourRequestRepository.GetAsync(filter: b => b.Status == 1 && b.EndDate.AddDays(3) <= DateTime.Now);
                    if (btgr.Any())
                    {
                        foreach (var item in btgr)
                        {

                            var order = (await _unitOfWork.OrderRepository.GetAsync(o => o.BookingTourRequestsId == item.BookingTourRequestId)).FirstOrDefault();
                            if (order != null)
                            {
                                var tour = await _unitOfWork.TourRepository.GetByIDAsync(item.TourId);
                                if (tour != null)
                                {
                                    var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(tour.TourGuideId);
                                    if (tourGuide != null)
                                    {
                                        item.Status = 3;
                                        await _unitOfWork.BookingTourRequestRepository.UpdateAsync(item);
                                        tourGuide.Balance += (order.OrderPrice * (decimal)0.7);
                                        await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                                        await _unitOfWork.SaveAsync();
                                    }
                                }
                            }
                        }
                        await Transaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await Transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        //public async Task RejectTimeOutBookingTourRequest()
        //{
        //    using (var Transaction = _unitOfWork.BeginTransaction())
        //    {
        //        try
        //        {
        //            var bookingRequests = await _unitOfWork.BookingTourRequestRepository.GetAllAsync(filter: b => b.RequestTimeOut < DateTime.UtcNow && b.Status == 0);
        //            if (bookingRequests.Any())
        //            {
        //                foreach (var item in bookingRequests)
        //                {
        //                    var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(item.CustomerId);
        //                    var order = (await _unitOfWork.OrderRepository.GetAsync(o => o.CustomerId == customer.CustomerId && o.BookingTourRequestsId == item.BookingTourRequestId)).FirstOrDefault();
        //                    if (order != null)
        //                    {
        //                        if (order.Status == 1)
        //                        {
        //                            customer.Balance += order.OrderPrice;
        //                            await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        //                        }
        //                    }
        //                    item.Status = 2;
        //                    await _unitOfWork.BookingTourRequestRepository.UpdateAsync(item);
        //                    var tour = await _unitOfWork.TourRepository.GetByIDAsync(item.TourId);
        //                    var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.TourGuideId == tour.TourGuideId)).FirstOrDefault();
        //                    if (tourGuide != null)
        //                    {
        //                        tourGuide.RejectedBookingCount++;
        //                        await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
        //                    }
        //                    await _unitOfWork.SaveAsync();
        //                    await Transaction.CommitAsync();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await Transaction.RollbackAsync();
        //            Console.WriteLine(ex.ToString());
        //        }
        //    }
        //}

        public async Task RejectTimeOutBookingTourRequest()
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingRequests = await _unitOfWork.BookingTourRequestRepository.GetAllAsync(filter: b => b.RequestTimeOut < DateTime.UtcNow && b.Status == 0);
                    if (bookingRequests.Any())
                    {
                        foreach (var item in bookingRequests)
                        {
                            // Cập nhật trạng thái của yêu cầu đặt tour
                            item.Status = 2;
                            await _unitOfWork.BookingTourRequestRepository.UpdateAsync(item);

                            // Lấy thông tin tour
                            var tour = await _unitOfWork.TourRepository.GetByIDAsync(item.TourId);
                            if (tour != null)
                            {
                                var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(tour.TourGuideId);
                                if (tourGuide != null)
                                {
                                    // Tăng số lần bị từ chối của hướng dẫn viên
                                    tourGuide.RejectedBookingCount++;
                                    await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                                }
                            }

                            await _unitOfWork.SaveAsync();
                        }
                        await transaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.ToString());
                }
            }
        }



    }
}
