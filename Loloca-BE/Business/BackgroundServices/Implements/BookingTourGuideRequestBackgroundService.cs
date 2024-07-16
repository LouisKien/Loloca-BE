using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.BackgroundServices.Implements
{
    public class BookingTourGuideRequestBackgroundService : IBookingTourGuideRequestBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingTourGuideRequestBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public async Task CompletedBookingTourGuideRequest()
        //{
        //    using (var Transaction = _unitOfWork.BeginTransaction())
        //    {
        //        try
        //        {
        //            var btgr = await _unitOfWork.BookingTourGuideRepository.GetAsync(filter: b => b.Status == 1 && b.EndDate.AddDays(3) <= DateTime.Now);
        //            if(btgr.Any())
        //            {
        //                foreach (var item in btgr)
        //                {
        //                    var order = (await _unitOfWork.OrderRepository.GetAsync(o => o.BookingTourGuideRequestId == item.BookingTourGuideRequestId)).FirstOrDefault();
        //                    if (order != null)
        //                    {
        //                        var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(item.TourGuideId);
        //                        if (tourGuide != null)
        //                        {
        //                            item.Status = 3;
        //                            await _unitOfWork.BookingTourGuideRepository.UpdateAsync(item);
        //                            tourGuide.Balance += (order.OrderPrice * (decimal) 0.7);
        //                            await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
        //                            await _unitOfWork.SaveAsync();
        //                        }
        //                    }
        //                }
        //                await Transaction.CommitAsync();
        //            }
        //        } catch (Exception ex)
        //        {
        //            await Transaction.RollbackAsync();
        //            throw new Exception(ex.Message);
        //        }
        //    }
        //}

        public async Task RejectTimeOutBookingTourGuideRequest()
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingRequests = await _unitOfWork.BookingTourGuideRepository.GetAllAsync(filter: b => b.RequestTimeOut < DateTime.Now && b.Status == 0);
                    if (bookingRequests.Any())
                    {
                        foreach (var item in bookingRequests)
                        {
                            //var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(item.CustomerId);
                            //var order = (await _unitOfWork.OrderRepository.GetAsync(o => o.CustomerId == customer.CustomerId && o.BookingTourGuideRequestId == item.BookingTourGuideRequestId)).FirstOrDefault();
                            //if (order != null)
                            //{
                            //    if (order.Status == 1)
                            //    {
                            //        customer.Balance += order.OrderPrice;
                            //        await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                            //    }
                            //}
                            item.Status = 2;
                            await _unitOfWork.BookingTourGuideRepository.UpdateAsync(item);
                            var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.TourGuideId == item.TourGuideId)).FirstOrDefault();
                            if(tourGuide != null)
                            {
                                tourGuide.RejectedBookingCount++;
                                await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                            }
                            await _unitOfWork.SaveAsync();
                        }
                        await Transaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await Transaction.RollbackAsync();
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
