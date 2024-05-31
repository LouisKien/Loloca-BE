using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Loloca_BE.Data.Repositories.Interfaces;
using System.Security.Claims;

namespace Loloca_BE.Business.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationAsync()
        {
            try
            {
                var list = await _unitOfWork.NotificationRepository.GetAsync();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while get notification.", ex);
            }
        }

        public async  Task<Notification> GetNotificationByIdAsync(int id)
        {
            try
            {
                var list = await _unitOfWork.NotificationRepository.GetByIDAsync(id);
                if (list == null)
                {
                    return null;
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while get notification", ex);
            }
        }

        //public async Task<bool> MarkAsRead(int notificationId)
        //{
        //    var notification = await _unitOfWork.NotificationRepository.GetByIDAsync(notificationId);
        //    if (notification == null)
        //    {
        //        throw new Exception("Không tìm thấy thông báo");
        //    }

        //    // Get the current user's ID from the HttpContext
        //    var currentUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (notification.UserId.ToString() != currentUserId)
        //    {
        //        throw new UnauthorizedAccessException("Bạn không có quyền đánh dấu thông báo này");
        //    }

        //    notification.IsRead = true;
        //    await _unitOfWork.NotificationRepository.UpdateAsync(notification);
        //    await _unitOfWork.SaveAsync();

        //    return true;
        //}

        public async Task<bool> MarkAsRead(int notificationId)
        {
            var notification = await _unitOfWork.NotificationRepository.GetByIDAsync(notificationId);
            if (notification == null)
            {
                throw new Exception("Không tìm thấy thông báo");
            }

            notification.IsRead = true;
            await _unitOfWork.NotificationRepository.UpdateAsync(notification);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<Notification>> GetNotificationByUserIdAndTypeUser(int userId, string userType)
        {
            try
            {
                var notifications = await _unitOfWork.NotificationRepository.GetAsync(n => n.UserId == userId);

                // Lọc kết quả trong bộ nhớ để sử dụng StringComparison.OrdinalIgnoreCase
                var filteredNotifications = notifications
                    .Where(n => n.UserType.Equals(userType, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return filteredNotifications;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting notifications.", ex);
            }
        }
    }
}
