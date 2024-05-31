using AutoMapper;
using Loloca_BE.Business.Models.PaymentRequestView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;

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
    }
}
