using Hangfire;
using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Data.Repositories.Interfaces;
using System.Linq;

namespace Loloca_BE.Business.BackgroundServices.Implements
{
    public class PaymentRequestBackgroundService : IPaymentRequestBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentRequestBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RejectExpiredPaymentRequest()
        {
            
            try
            {
                var pr = await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.RequestDate.AddDays(7) < DateTime.Now && p.Status == 0);
                if (pr.Any())
                {
                    foreach (var item in pr)
                    {
                        if (item.Type == 1)
                        {
                            item.Status = 2;
                            await _unitOfWork.PaymentRequestRepository.UpdateAsync(item);
                            await _unitOfWork.SaveAsync();
                        }
                        else if (item.Type == 2)
                        {
                            using (var Transaction = _unitOfWork.BeginTransaction())
                            {
                                try
                                {
                                    item.Status = 2;
                                    await _unitOfWork.PaymentRequestRepository.UpdateAsync(item);
                                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(item.AccountId);
                                    if (account.Role == 2)
                                    {
                                        var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == item.Account.AccountId)).FirstOrDefault();
                                        if (tourGuide != null)
                                        {
                                            if (tourGuide.Balance == null)
                                            {
                                                tourGuide.Balance = 0;
                                            }
                                            tourGuide.Balance += item.Amount;
                                            await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                                            await _unitOfWork.SaveAsync();
                                        }
                                    }
                                    else if (account.Role == 3)
                                    {
                                        var customer = (await _unitOfWork.CustomerRepository.GetAsync(t => t.AccountId == item.Account.AccountId)).FirstOrDefault();
                                        if (customer != null)
                                        {
                                            if (customer.Balance == null)
                                            {
                                                customer.Balance = 0;
                                            }
                                            customer.Balance += item.Amount;
                                            await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                                            await _unitOfWork.SaveAsync();
                                        }
                                    }
                                    await Transaction.CommitAsync();
                                } catch (Exception ex)
                                {
                                    await Transaction.RollbackAsync();
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
