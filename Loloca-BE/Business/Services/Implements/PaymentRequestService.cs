using AutoMapper;
using Loloca_BE.Business.Models.PaymentRequestView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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
            using (var transaction = _unitOfWork.BeginTransaction())
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

                    // Tạo thông báo cho người dùng
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(deposit.AccountId);
                    if (account != null)
                    {
                        string userType = account.Role switch
                        {
                            1 => "Admin",
                            2 => "TourGuide",
                            3 => "Customer"
                        };

                        int? userId = null;

                        if (userType == "Customer")
                        {
                            var customer = await _unitOfWork.CustomerRepository.FindAsync(c => c.AccountId == deposit.AccountId);
                            var customerEntity = customer.SingleOrDefault(); // Lấy đối tượng đơn lẻ
                            userId = customerEntity?.CustomerId;
                            Debug.WriteLine($"CustomerEntity: {customerEntity}");
                        }
                        else if (userType == "TourGuide")
                        {
                            var tourGuide = await _unitOfWork.TourGuideRepository.FindAsync(t => t.AccountId == deposit.AccountId);
                            var tourGuideEntity = tourGuide.SingleOrDefault(); // Lấy đối tượng đơn lẻ
                            userId = tourGuideEntity?.TourGuideId;
                            Debug.WriteLine($"TourGuideEntity: {tourGuideEntity}");
                        }

                        Debug.WriteLine($"UserId: {userId}");

                        if (userId != null)
                        {
                            var notification = new Notification
                            {
                                UserId = userId.Value,
                                UserType = userType,
                                Title = "Yêu cầu nạp tiền mới",
                                Message = $"Yêu cầu nạp tiền của bạn với số tiền {deposit.Amount} đã được tạo.",
                                IsRead = false,
                                CreatedAt = DateTime.Now
                            };

                            await _unitOfWork.NotificationRepository.InsertAsync(notification);
                            Debug.WriteLine("Notification created successfully.");
                        }
                        else
                        {
                            Debug.WriteLine("UserId is null, notification not created.");
                        }
                    }

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    // Log lỗi chi tiết hơn từ InnerException
                    throw new Exception("An error occurred while saving the entity changes: " + dbEx.InnerException?.Message, dbEx);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }






        public async Task UpdateStatusDepositAsync(UpdateDepositStatusView updateDepositStatusView)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var paymentRequests = await _unitOfWork.PaymentRequestRepository.GetAsync(p => p.PaymentId == updateDepositStatusView.PaymentRequestId, includeProperties: "Account");
                    var paymentRequest = paymentRequests.FirstOrDefault();
                    if (paymentRequest == null)
                    {
                        throw new Exception("Không tìm thấy yêu cầu nạp tiền.");
                    }
                    if (paymentRequest.Type != 1)
                    {
                        throw new Exception("Loại giao dịch không phù hợp.");
                    }
                    if (paymentRequest.Status != 0)
                    {
                        throw new Exception("Yêu cầu đã được xử lý trước đó.");
                    }

                    string userType;
                    int userId;

                    if (paymentRequest.Account.Role == 2)
                    {
                        var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == paymentRequest.Account.AccountId)).FirstOrDefault();
                        if (tourGuide == null)
                        {
                            throw new Exception("Không tìm thấy thông tin hướng dẫn viên.");
                        }
                        userType = "TourGuide";
                        userId = tourGuide.TourGuideId;
                    }
                    else if (paymentRequest.Account.Role == 3)
                    {
                        var customer = (await _unitOfWork.CustomerRepository.GetAsync(t => t.AccountId == paymentRequest.Account.AccountId)).FirstOrDefault();
                        if (customer == null)
                        {
                            throw new Exception("Không tìm thấy thông tin khách hàng.");
                        }
                        userType = "Customer";
                        userId = customer.CustomerId;
                    }
                    else if (paymentRequest.Account.Role == 1)
                    {
                        throw new Exception("Admin không thể tự phê duyệt yêu cầu của mình.");
                    }
                    else
                    {
                        throw new Exception("Vai trò tài khoản không hợp lệ.");
                    }

                    paymentRequest.Status = updateDepositStatusView.Status;
                    await _unitOfWork.PaymentRequestRepository.UpdateAsync(paymentRequest);

                    var notificationTitle = updateDepositStatusView.Status == 1 ? "Yêu cầu nạp tiền đã được chấp thuận" : "Yêu cầu nạp tiền bị từ chối";
                    var notificationMessage = updateDepositStatusView.Status == 1
                        ? $"Yêu cầu nạp tiền của bạn với số tiền {paymentRequest.Amount} đã được chấp nhận."
                        : $"Yêu cầu nạp tiền của bạn với số tiền {paymentRequest.Amount} đã bị từ chối.";

                    var notification = new Notification
                    {
                        UserId = userId,
                        UserType = userType,
                        Title = notificationTitle,
                        Message = notificationMessage,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.NotificationRepository.InsertAsync(notification);

                    if (updateDepositStatusView.Status == 1)
                    {
                        if (userType == "TourGuide")
                        {
                            var tourGuide = await _unitOfWork.TourGuideRepository.GetByIDAsync(userId);
                            if (tourGuide.Balance == null)
                            {
                                tourGuide.Balance = 0;
                            }
                            tourGuide.Balance += paymentRequest.Amount;
                            await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                        }
                        else if (userType == "Customer")
                        {
                            var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(userId);
                            if (customer.Balance == null)
                            {
                                customer.Balance = 0;
                            }
                            customer.Balance += paymentRequest.Amount;
                            await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                        }
                    }

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Cannot update status deposit", ex);
                }
            }
        }





        // ----------------------------------------- WITHDRAWAL --------------------------------------------
        public async Task<int> SendWithdrawalRequest(WithdrawalView withdrawalView)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(withdrawalView.AccountId);
                    if (account != null)
                    {
                        if (account.Role == 1)
                        {
                            return -1; // Admin role can't send request
                        }

                        int? userId = null;
                        string userType = account.Role switch
                        {
                            2 => "TourGuide",
                            3 => "Customer"
                        };

                        if (account.Role == 2)
                        {
                            var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == account.AccountId)).FirstOrDefault();
                            if (tourGuide != null)
                            {
                                if (tourGuide.Balance < withdrawalView.Amount)
                                {
                                    return -2; // Out of balance
                                }
                                else
                                {
                                    tourGuide.Balance -= withdrawalView.Amount;
                                    await _unitOfWork.TourGuideRepository.UpdateAsync(tourGuide);
                                    userId = tourGuide.TourGuideId;
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
                                    return -2; // Out of balance
                                }
                                else
                                {
                                    customer.Balance -= withdrawalView.Amount;
                                    await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                                    userId = customer.CustomerId;
                                }
                            }
                        }

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

                        // Create notification if userId is not null
                        if (userId != null)
                        {
                            var notification = new Notification
                            {
                                UserId = userId.Value,
                                UserType = userType,
                                Title = "Yêu cầu rút tiền mới",
                                Message = $"Yêu cầu rút tiền của bạn với số tiền {withdrawal.Amount} đã được tạo.",
                                IsRead = false,
                                CreatedAt = DateTime.Now
                            };

                            await _unitOfWork.NotificationRepository.InsertAsync(notification);
                        }

                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return 1; // Successful
                    }
                    return 0; // Account not found
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
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

        public async Task UpdateStatusWithdrawalAsync(UpdateWithdrawalStatusView updateWithdrawalStatusView)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var paymentRequests = await _unitOfWork.PaymentRequestRepository.GetAsync(p => p.PaymentId == updateWithdrawalStatusView.PaymentRequestId, includeProperties: "Account");
                    var paymentRequest = paymentRequests.FirstOrDefault();
                    if (paymentRequest == null)
                    {
                        throw new Exception("Không tìm thấy yêu cầu rút tiền.");
                    }
                    if (paymentRequest.Type != 2)
                    {
                        throw new Exception("Loại giao dịch không phù hợp.");
                    }
                    if (paymentRequest.Status != 0)
                    {
                        throw new Exception("Yêu cầu đã được xử lý trước đó.");
                    }

                    string userType;
                    int userId;

                    if (paymentRequest.Account.Role == 2)
                    {
                        var tourGuide = (await _unitOfWork.TourGuideRepository.GetAsync(t => t.AccountId == paymentRequest.Account.AccountId)).FirstOrDefault();
                        if (tourGuide == null)
                        {
                            throw new Exception("Không tìm thấy thông tin hướng dẫn viên.");
                        }
                        userType = "TourGuide";
                        userId = tourGuide.TourGuideId;
                    }
                    else if (paymentRequest.Account.Role == 3)
                    {
                        var customer = (await _unitOfWork.CustomerRepository.GetAsync(t => t.AccountId == paymentRequest.Account.AccountId)).FirstOrDefault();
                        if (customer == null)
                        {
                            throw new Exception("Không tìm thấy thông tin khách hàng.");
                        }
                        userType = "Customer";
                        userId = customer.CustomerId;
                    }
                    else if (paymentRequest.Account.Role == 1)
                    {
                        throw new Exception("Admin không thể tự phê duyệt yêu cầu của mình.");
                    }
                    else
                    {
                        throw new Exception("Vai trò tài khoản không hợp lệ.");
                    }

                    paymentRequest.Status = 1; // Đặt trạng thái là 1
                    await _unitOfWork.PaymentRequestRepository.UpdateAsync(paymentRequest);

                    var notificationTitle = "Yêu cầu rút tiền đã được chấp thuận";
                    var notificationMessage = $"Yêu cầu rút tiền của bạn với số tiền {paymentRequest.Amount} đã được chấp nhận.";

                    var notification = new Notification
                    {
                        UserId = userId,
                        UserType = userType,
                        Title = notificationTitle,
                        Message = notificationMessage,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.NotificationRepository.InsertAsync(notification);

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Không thể cập nhật trạng thái yêu cầu rút tiền", ex);
                }
            }
        }











    }
}
