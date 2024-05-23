using AutoMapper;
using Loloca_BE.Business.Models.CustomerView;
using Loloca_BE.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace Loloca_BE.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public CustomerService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task UpdateCustomerInfo(int customerId, UpdateProfile model)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(customerId);
                if (customer == null)
                {
                    throw new Exception("Customer not found");
                }

                _mapper.Map(model, customer);

                await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                // Add logging or other error handling as needed
                throw new Exception("Error updating customer info", ex);
            }
        }

        public async Task<bool> ChangeCustomerPassword(int customerId, ChangePassword model)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(customerId);
                if (account == null)
                {
                    throw new Exception("Không tìm thấy khách hàng");
                }

                // Kiểm tra vai trò của tài khoản
                if (account.Role != 3)
                {
                    throw new Exception("Không được phép thay đổi mật khẩu");
                }

                if (!await _authService.VerifyPassword(model.OldPassword, account.HashedPassword))
                {
                    throw new Exception("Mật khẩu hiện tại không đúng");
                }

                account.HashedPassword = await _authService.HashPassword(model.NewPassword);

                await _unitOfWork.AccountRepository.UpdateAsync(account);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Thêm logging hoặc xử lý lỗi khác nếu cần
                throw new Exception("Lỗi khi thay đổi mật khẩu khách hàng", ex);
            }
        }

    }
}
