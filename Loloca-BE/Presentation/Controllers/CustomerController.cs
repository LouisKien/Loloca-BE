using Loloca_BE.Business.Models.CustomerView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
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
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
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
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/customer/update-avatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] IFormFile file, [FromForm] int CustomerId)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest("No file provided.");
                }

                await _customerService.UploadAvatarAsync(file, CustomerId);

                return Ok("Avatar uploaded successfully!");
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/customer")]
        public async Task<IActionResult> GetListCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var totalPage = await _customerService.GetTotalPage(pageSize);
                if (page > totalPage)
                {
                    return NotFound("This page does not exist.");
                }
                var customersView = await _customerService.GetCustomers(page, pageSize);
                return Ok(new { customersView, totalPage});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("api/v1/customer/")]
        public async Task<IActionResult> GetCustomerById([FromQuery] int customerId)
        {
            try
            {
                var customer = await _customerService.GetCustomerById(customerId);
                if (customer == null)
                {
                    return NotFound("This customer does not exist");
                }
                if(customer.AccountStatus != 1)
                {
                    return NotFound("This customer is currently not available");
                }
                return Ok(customer);
            } catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("api/v1/customer/private")]
        public async Task<IActionResult> GetCustomerByIdPrivate([FromQuery] int customerId)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdPrivate(customerId);
                if (customer == null)
                {
                    return NotFound("This customer does not exist");
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }
    }
}
