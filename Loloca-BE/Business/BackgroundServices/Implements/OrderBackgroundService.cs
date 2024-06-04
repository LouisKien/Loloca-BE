using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.BackgroundServices.Implements
{
    public class OrderBackgroundService : IOrderBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RejectExpiredOrder()
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingRequests = await _unitOfWork.OrderRepository.GetAllAsync(filter: o => o.CreateAt.AddHours(72) < DateTime.Now && o.Status == 0);
                    if (bookingRequests.Any())
                    {
                        foreach (var item in bookingRequests)
                        {
                            item.Status = 2;
                            await _unitOfWork.OrderRepository.UpdateAsync(item);
                            var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(item.CustomerId);
                            customer.CanceledBookingCount++;
                            await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                            if(item.BookingTourGuideRequestId != null)
                            {
                                var btgr = await _unitOfWork.BookingTourGuideRepository.GetByIDAsync(item.BookingTourGuideRequestId);
                                btgr.Status = 2;
                                await _unitOfWork.BookingTourGuideRepository.UpdateAsync(btgr);
                            } else if (item.BookingTourRequestsId != null)
                            {
                                var btr = await _unitOfWork.BookingTourRequestRepository.GetByIDAsync(item.BookingTourRequestsId);
                                btr.Status = 2;
                                await _unitOfWork.BookingTourRequestRepository.UpdateAsync(btr);
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
