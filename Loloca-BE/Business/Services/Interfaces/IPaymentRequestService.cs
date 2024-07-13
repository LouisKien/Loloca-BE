using Loloca_BE.Business.Models.PaymentRequestView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface IPaymentRequestService
    {
        // ----------------------------------------- DEPOSIT --------------------------------------------
        Task SendDepositRequest(DepositRequestView depositView);
        Task<List<GetDepositView>> GetAllDepositRequest(int? status);
        Task<GetDepositView?> GetDepositById(int PaymentRequestId);
        Task<List<GetDepositView>> GetDepositByCustomerId(int customerId, int? status);
        Task<List<GetDepositView>> GetDepositByTourGuideId(int tourGuideId, int? status);
        //Task<int> UpdateStatusDeposit(int paymentRequestId, int status);
        Task UpdateStatusDepositAsync(UpdateDepositStatusView updateDepositStatusView);

        // ----------------------------------------- WITHDRAWAL --------------------------------------------
        Task<int> SendWithdrawalRequest(WithdrawalView withdrawalView);
        Task<List<GetWithdrawalView>> GetAllWithdrawalRequest(int? status);
        Task<GetWithdrawalView?> GetAllWithdrawalById(int PaymentRequestId);
        Task<List<GetWithdrawalView>> GetAllWithdrawalByCustomerId(int CustomerId, int? status);
        Task<List<GetWithdrawalView>> GetAllWithdrawalByTourGuideId(int TourGuideId, int? status);
        Task UpdateStatusWithdrawalAsync(UpdateWithdrawalStatusView updateWithdrawalStatusView);
    }
}
