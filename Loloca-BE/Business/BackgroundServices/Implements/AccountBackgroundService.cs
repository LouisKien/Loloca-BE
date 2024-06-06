using Loloca_BE.Business.BackgroundServices.Interfaces;
using Loloca_BE.Data.Repositories.Interfaces;

namespace Loloca_BE.Business.BackgroundServices.Implements
{
    public class AccountBackgroundService : IAccountBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LockSpammedCustomerAccount()
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var customer = await _unitOfWork.CustomerRepository.GetAllAsync(filter: c => c.CanceledBookingCount >= 3);
                    if (customer.Any())
                    {
                        foreach (var item in customer)
                        {
                            var account = await _unitOfWork.AccountRepository.GetByIDAsync(item.AccountId);
                            account.Status = 2;
                            account.ReleaseDate = DateTime.Now.AddDays(90);
                            await _unitOfWork.AccountRepository.UpdateAsync(account);
                            item.CanceledBookingCount = 0;
                            await _unitOfWork.CustomerRepository.UpdateAsync(item);
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

        public async Task LockSpammedTourGuideAccount()
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var tourGuide = await _unitOfWork.TourGuideRepository.GetAllAsync(filter: t => t.RejectedBookingCount >= 5);
                    if (tourGuide.Any())
                    {
                        foreach(var item in tourGuide)
                        {
                            var account = await _unitOfWork.AccountRepository.GetByIDAsync(item.AccountId);
                            account.Status = 2;
                            account.ReleaseDate = DateTime.Now.AddDays(90);
                            item.RejectedBookingCount = 0;
                            item.Status = 0;
                            await _unitOfWork.TourGuideRepository.UpdateAsync(item);
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

        public async Task UnlockSpammedCustomerAccount()
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetAsync(filter: a => a.Status == 3 && a.ReleaseDate < DateTime.UtcNow && a.Role == 3);
                if (account.Any())
                {
                    foreach (var item in account)
                    {
                        item.Status = 1;
                        item.ReleaseDate = null;
                        await _unitOfWork.AccountRepository.UpdateAsync(item);
                        await _unitOfWork.SaveAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task UnlockSpammedTourGuideAccount()
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var account = await _unitOfWork.AccountRepository.GetAsync(filter: a => a.Status == 3 && a.ReleaseDate < DateTime.UtcNow && a.Role == 2);
                    if (account.Any())
                    {
                        foreach (var item in account)
                        {
                            var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == item.AccountId)).FirstOrDefault();
                            tourGuide.Status = 1;
                            await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                            item.Status = 1;
                            item.ReleaseDate = null;
                            await _unitOfWork.AccountRepository.UpdateAsync(item);
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
