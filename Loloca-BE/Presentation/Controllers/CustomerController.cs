using Loloca_BE.Business.Models.CustomerView;
using Loloca_BE.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpPost("update-info")]
        public async Task<IActionResult> UpdateCustomerInfo(int customerId, [FromBody] UpdateProfile model)
        {
            try
            {
                await _customerService.UpdateCustomerInfo(customerId, model);
                return Ok("Cập nhật thông tin thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(int accountId, [FromBody] ChangePassword model)
        {
            try
            {
                var success = await _customerService.ChangeCustomerPassword(accountId, model);
                if (success)
                {
                    return Ok("Đổi mật khẩu thành công");
                }
                else
                {
                    return BadRequest("Đổi mật khẩu không thành công");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }



    }
}
