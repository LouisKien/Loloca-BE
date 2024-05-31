using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Data.Entities;

namespace Loloca_BE.Business.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> MarkAsRead(int notificationId);
        Task<IEnumerable<Notification>> GetAllNotificationAsync();
        Task<Notification> GetNotificationByIdAsync(int id);
        Task<IEnumerable<Notification>> GetNotificationByUserIdAndTypeUser(int userId, string userType);
    }
}
