
using AutoMapper;
using Loloca_BE.Data.Repositories;

namespace Loloca_BE.Business.Services
{
    public class AdminService : IAdminService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> LockAccount(int accountId)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (account == null)
                {
                    throw new Exception("Không tìm thấy tài khoản");
                }

                account.Status = 0; // giả định rằng 0 là trạng thái khóa

                await _unitOfWork.AccountRepository.UpdateAsync(account);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Thêm logging hoặc xử lý lỗi khác nếu cần
                throw new Exception("Lỗi khi khóa tài khoản", ex);
            }
        }
    }
    
}
