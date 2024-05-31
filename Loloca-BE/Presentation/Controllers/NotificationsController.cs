using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var result = await _notificationService.MarkAsRead(id);
                if (result)
                {
                    return Ok("Thông báo đã được đánh dấu là đã đọc");
                }
                else
                {
                    return BadRequest("Không thể đánh dấu thông báo");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotification()
        {
            try
            {
                var noti = await _notificationService.GetAllNotificationAsync();
                return Ok(noti);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            try
            {
                var noti = await _notificationService.GetNotificationByIdAsync(id);
                if (noti == null)
                {
                    return NotFound();
                }
                return Ok(noti);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}/{userType}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByUserIdAndTypeUser(int userId, string userType)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationByUserIdAndTypeUser(userId, userType);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }

}
