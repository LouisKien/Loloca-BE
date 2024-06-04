using Loloca_BE.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("lock-account")]
        public async Task<IActionResult> LockAccount(int accountId)
        {
            try
            {
                var result = await _adminService.LockAccount(accountId);
                if (result)
                {
                    return Ok("Khóa tài khoản thành công");
                }
                else
                {
                    return BadRequest("Khóa tài khoản không thành công");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }
    }
}
