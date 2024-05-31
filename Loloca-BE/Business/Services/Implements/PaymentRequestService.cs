using AutoMapper;
using Loloca_BE.Business.Models.PaymentRequestView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using System.Security.Principal;

namespace Loloca_BE.Business.Services.Implements
{
    public class PaymentRequestService : IPaymentRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentRequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ----------------------------------------- DEPOSIT --------------------------------------------
        public async Task<List<GetDepositView>> GetAllDepositRequest(int? status)
        {
            try
            {
                List<PaymentRequest> deposits;
                if (status == null)
                {
                    deposits = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 1, includeProperties: "Account")).ToList();
                } else
                {
                    deposits = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 1 && p.Status == status, includeProperties: "Account")).ToList();
                }
                List<GetDepositView> getDepositViews = new List<GetDepositView>();
                foreach (var deposit in deposits)
                {
                    var depositView = new GetDepositView
                    {
                        AccountId = deposit.AccountId,
                        Amount = deposit.Amount,
                        Email = deposit.Account.Email,
                        PaymentId = deposit.PaymentId,
                        RequestDate = deposit.RequestDate,
                        Status = deposit.Status,
                        TransactionCode = deposit.TransactionCode
                    };
                    getDepositViews.Add(depositView);
                }
                return getDepositViews;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetDepositView>> GetDepositByCustomerId(int customerId, int? status)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(customerId);
                List<PaymentRequest> deposits;
                if (status == null)
                {
                    deposits = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 1 && p.AccountId == customer.AccountId, includeProperties: "Account")).ToList();
                }
                else
                {
                    deposits = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 1 && p.Status == status && p.AccountId == customer.AccountId, includeProperties: "Account")).ToList();
                }
                List<GetDepositView> getDepositViews = new List<GetDepositView>();
                foreach (var deposit in deposits)
                {
                    var depositView = new GetDepositView
                    {
                        AccountId = deposit.AccountId,
                        Amount = deposit.Amount,
                        Email = deposit.Account.Email,
                        PaymentId = deposit.PaymentId,
                        RequestDate = deposit.RequestDate,
                        Status = deposit.Status,
                        TransactionCode = deposit.TransactionCode
                    };
                    getDepositViews.Add(depositView);
                }
                return getDepositViews;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetDepositView?> GetDepositById(int PaymentRequestId)
        {
            try
            {
                var deposit = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 1 && p.PaymentId == PaymentRequestId, includeProperties: "Account")).FirstOrDefault();
                if(deposit == null)
                {
                    return null;
                }
                var depositView = new GetDepositView
                {
                    AccountId = deposit.AccountId,
                    Amount = deposit.Amount,
                    Email = deposit.Account.Email,
                    PaymentId = deposit.PaymentId,
                    RequestDate = deposit.RequestDate,
                    Status = deposit.Status,
                    TransactionCode = deposit.TransactionCode
                };
                return depositView;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetDepositView>> GetDepositByTourGuideId(int tourGuideId, int? status)
        {
            try
            {
                var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(tourGuideId);
                List<PaymentRequest> deposits;
                if (status == null)
                {
                    deposits = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 1 && p.AccountId == tourGuide.AccountId, includeProperties: "Account")).ToList();
                }
                else
                {
                    deposits = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 1 && p.Status == status && p.AccountId == tourGuide.AccountId, includeProperties: "Account")).ToList();
                }
                List<GetDepositView> getDepositViews = new List<GetDepositView>();
                foreach (var deposit in deposits)
                {
                    var depositView = new GetDepositView
                    {
                        AccountId = deposit.AccountId,
                        Amount = deposit.Amount,
                        Email = deposit.Account.Email,
                        PaymentId = deposit.PaymentId,
                        RequestDate = deposit.RequestDate,
                        Status = deposit.Status,
                        TransactionCode = deposit.TransactionCode
                    };
                    getDepositViews.Add(depositView);
                }
                return getDepositViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendDepositRequest(DepositRequestView depositView)
        {
            try
            {
                var deposit = new PaymentRequest
                {
                    AccountId = depositView.AccountId,
                    Amount = depositView.Amount,
                    Type = 1,
                    TransactionCode = depositView.TransactionCode,
                    RequestDate = DateTime.Now,
                    Status = 0
                };
                await _unitOfWork.PaymentRequestRepository.InsertAsync(deposit);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateStatusDeposit(int paymentRequestId, int status)
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var pr = (await _unitOfWork.PaymentRequestRepository.GetAsync(p => p.PaymentId == paymentRequestId, includeProperties: "Account")).FirstOrDefault();
                    if (pr == null)
                    {
                        return -1;
                    }
                    if (pr.Type != 1)
                    {
                        return -2;
                    }
                    if (status == 1 && pr.Status == 0)
                    {
                        pr.Status = status;
                        await _unitOfWork.PaymentRequestRepository.UpdateAsync(pr);

                        if (pr.Account.Role == 2)
                        {
                            var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == pr.Account.AccountId)).FirstOrDefault();
                            if (tourGuide != null)
                            {
                                if (tourGuide.Balance == null)
                                {
                                    tourGuide.Balance = 0;
                                }
                                tourGuide.Balance += pr.Amount;
                                await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                                await Transaction.CommitAsync();
                                return 1;
                            }
                        }
                        else if (pr.Account.Role == 3)
                        {
                            var customer = (await _unitOfWork.CustomerRepository.GetAsync(t => t.AccountId == pr.Account.AccountId)).FirstOrDefault();
                            if (customer != null)
                            {
                                if (customer.Balance == null)
                                {
                                    customer.Balance = 0;
                                }
                                customer.Balance += pr.Amount;
                                await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                                await Transaction.CommitAsync();
                                return 1;
                            }
                        }
                        else if (pr.Account.Role == 1)
                        {
                            throw new Exception("Admin role can't approve their request");
                        }
                    }
                    else if (status == 2 && pr.Status == 0)
                    {
                        pr.Status = status;
                        await _unitOfWork.PaymentRequestRepository.UpdateAsync(pr);
                        await Transaction.CommitAsync();
                        return 2;
                    }
                    else
                    {
                        return 0;
                    }
                    return -1;
                }
                catch (Exception ex)
                {
                    await Transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        // ----------------------------------------- WITHDRAWAL --------------------------------------------
        public async Task<int> SendWithdrawalRequest(WithdrawalView withdrawalView)
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(withdrawalView.AccountId);
                    if (account != null)
                    {
                        if (account.Role == 1)
                        {
                            return -1;
                        }
                        if (account.Role == 2)
                        {
                            var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == account.AccountId)).FirstOrDefault();
                            if (tourGuide != null)
                            {
                                if (tourGuide.Balance < withdrawalView.Amount)
                                {
                                    return -2;
                                }
                                else
                                {
                                    tourGuide.Balance -= withdrawalView.Amount;
                                    await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                                    var withdrawal = new PaymentRequest
                                    {
                                        AccountId = withdrawalView.AccountId,
                                        Amount = withdrawalView.Amount,
                                        Type = 2,
                                        BankAccount = withdrawalView.BankAccount,
                                        Bank = withdrawalView.Bank,
                                        RequestDate = DateTime.Now,
                                        Status = 0
                                    };
                                    await _unitOfWork.PaymentRequestRepository.InsertAsync(withdrawal);
                                    await Transaction.CommitAsync();
                                    return 1;
                                }
                            }
                        }
                        else if (account.Role == 3)
                        {
                            var customer = (await _unitOfWork.CustomerRepository.GetAsync(t => t.AccountId == account.AccountId)).FirstOrDefault();
                            if (customer != null)
                            {
                                if (customer.Balance < withdrawalView.Amount)
                                {
                                    return -2;
                                }
                                else
                                {
                                    customer.Balance -= withdrawalView.Amount;
                                    await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                                    var withdrawal = new PaymentRequest
                                    {
                                        AccountId = withdrawalView.AccountId,
                                        Amount = withdrawalView.Amount,
                                        Type = 2,
                                        BankAccount = withdrawalView.BankAccount,
                                        Bank = withdrawalView.Bank,
                                        RequestDate = DateTime.Now,
                                        Status = 0
                                    };
                                    await _unitOfWork.PaymentRequestRepository.InsertAsync(withdrawal);
                                    await Transaction.CommitAsync();
                                    return 1;
                                }
                            }
                        }
                    }
                    return 0;
                } catch (Exception ex)
                {
                    await Transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<GetWithdrawalView>> GetAllWithdrawalRequest(int? status)
        {
            try
            {
                List<PaymentRequest> withdrawals;
                if (status == null)
                {
                    withdrawals = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 2, includeProperties: "Account")).ToList();
                }
                else
                {
                    withdrawals = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 2 && p.Status == status, includeProperties: "Account")).ToList();
                }
                List<GetWithdrawalView> getWithdrawalViews = new List<GetWithdrawalView>();
                foreach (var withdrawal in withdrawals)
                {
                    var withdrawalView = new GetWithdrawalView
                    {
                        AccountId = withdrawal.AccountId,
                        Amount = withdrawal.Amount,
                        Email = withdrawal.Account.Email,
                        PaymentId = withdrawal.PaymentId,
                        RequestDate = withdrawal.RequestDate,
                        Status = withdrawal.Status,
                        BankAccount = withdrawal.BankAccount,
                        Bank = withdrawal.Bank
                    };
                    getWithdrawalViews.Add(withdrawalView);
                }
                return getWithdrawalViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetWithdrawalView?> GetAllWithdrawalById(int PaymentRequestId)
        {
            try
            {
                var withdrawal = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 2 && p.PaymentId == PaymentRequestId, includeProperties: "Account")).FirstOrDefault();
                if(withdrawal == null)
                {
                    return null;
                }
                var withdrawalView = new GetWithdrawalView
                {
                    AccountId = withdrawal.AccountId,
                    Amount = withdrawal.Amount,
                    Email = withdrawal.Account.Email,
                    PaymentId = withdrawal.PaymentId,
                    RequestDate = withdrawal.RequestDate,
                    Status = withdrawal.Status,
                    BankAccount = withdrawal.BankAccount,
                    Bank = withdrawal.Bank
                };
                return withdrawalView;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetWithdrawalView>> GetAllWithdrawalByCustomerId(int CustomerId, int? status)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(CustomerId);
                List<PaymentRequest> withdrawals;
                if (status == null)
                {
                    withdrawals = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 2 && p.AccountId == customer.AccountId, includeProperties: "Account")).ToList();
                }
                else
                {
                    withdrawals = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 2 && p.Status == status && p.AccountId == customer.AccountId, includeProperties: "Account")).ToList();
                }
                List<GetWithdrawalView> getWithdrawalViews = new List<GetWithdrawalView>();
                foreach (var withdrawal in withdrawals)
                {
                    var withdrawalView = new GetWithdrawalView
                    {
                        AccountId = withdrawal.AccountId,
                        Amount = withdrawal.Amount,
                        Email = withdrawal.Account.Email,
                        PaymentId = withdrawal.PaymentId,
                        RequestDate = withdrawal.RequestDate,
                        Status = withdrawal.Status,
                        BankAccount = withdrawal.BankAccount,
                        Bank = withdrawal.Bank
                    };
                    getWithdrawalViews.Add(withdrawalView);
                }
                return getWithdrawalViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<GetWithdrawalView>> GetAllWithdrawalByTourGuideId(int TourGuideId, int? status)
        {
            try
            {
                var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(TourGuideId);
                List<PaymentRequest> withdrawals;
                if (status == null)
                {
                    withdrawals = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 2 && p.AccountId == tourGuide.AccountId, includeProperties: "Account")).ToList();
                }
                else
                {
                    withdrawals = (await _unitOfWork.PaymentRequestRepository.GetAsync(filter: p => p.Type == 2 && p.Status == status && p.AccountId == tourGuide.AccountId, includeProperties: "Account")).ToList();
                }
                List<GetWithdrawalView> getWithdrawalViews = new List<GetWithdrawalView>();
                foreach (var withdrawal in withdrawals)
                {
                    var withdrawalView = new GetWithdrawalView
                    {
                        AccountId = withdrawal.AccountId,
                        Amount = withdrawal.Amount,
                        Email = withdrawal.Account.Email,
                        PaymentId = withdrawal.PaymentId,
                        RequestDate = withdrawal.RequestDate,
                        Status = withdrawal.Status,
                        BankAccount = withdrawal.BankAccount,
                        Bank = withdrawal.Bank
                    };
                    getWithdrawalViews.Add(withdrawalView);
                }
                return getWithdrawalViews;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> UpdateStatusWithdrawal(int paymentRequestId, int status)
        {
            using (var Transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var pr = (await _unitOfWork.PaymentRequestRepository.GetAsync(p => p.PaymentId == paymentRequestId, includeProperties: "Account")).FirstOrDefault();
                    if (pr == null)
                    {
                        return -1;
                    }
                    if (pr.Type != 2)
                    {
                        return -2;
                    }
                    if (status == 1 && pr.Status == 0)
                    {
                        pr.Status = status;
                        await _unitOfWork.PaymentRequestRepository.UpdateAsync(pr);
                        await Transaction.CommitAsync();
                        return 1;
                    }
                    else if (status == 2 && pr.Status == 0)
                    {
                        pr.Status = status;
                        await _unitOfWork.PaymentRequestRepository.UpdateAsync(pr);

                        if (pr.Account.Role == 2)
                        {
                            var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == pr.Account.AccountId)).FirstOrDefault();
                            if (tourGuide != null)
                            {
                                if (tourGuide.Balance == null)
                                {
                                    tourGuide.Balance = 0;
                                }
                                tourGuide.Balance += pr.Amount;
                                await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                                await Transaction.CommitAsync();
                                return 2;
                            }
                        }
                        else if (pr.Account.Role == 3)
                        {
                            var customer = (await _unitOfWork.CustomerRepository.GetAsync(t => t.AccountId == pr.Account.AccountId)).FirstOrDefault();
                            if (customer != null)
                            {
                                if (customer.Balance == null)
                                {
                                    customer.Balance = 0;
                                }
                                customer.Balance += pr.Amount;
                                await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                                await Transaction.CommitAsync();
                                return 2;
                            }
                        }
                    }
                    else
                    {
                        return 0;
                    }
                    return -3;
                }
                catch (Exception ex)
                {
                    await Transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
