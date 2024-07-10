using Loloca_BE.Business.Models.CustomerView;
using Loloca_BE.Business.Services.Implements;
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
        private readonly IAuthorizeService _authorizeService;
        public CustomerController(ICustomerService customerService, IAuthorizeService authorizeService)
        {
            _customerService = customerService;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("update-info/{customerId}")]
        public async Task<IActionResult> UpdateCustomerInfo(int customerId, [FromBody] UpdateProfile model)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    await _customerService.UpdateCustomerInfo(customerId, model);
                    return Ok("Cập nhật thông tin thành công");
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(int accountId, [FromBody] ChangePassword model)
        {
            try
            {
                var accountIdJwt = User.FindFirst("AccountId")?.Value;
                if (accountIdJwt == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByAccountId(accountId, int.Parse(accountIdJwt));
                if (checkAuthorize.isUser)
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
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("update-avatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] List<IFormFile> files, [FromForm] int CustomerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByCustomerId(CustomerId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    if (!files.Any())
                    {
                        return BadRequest("No file provided.");
                    }
                    var file = files.FirstOrDefault();
                    if (file == null)
                    {
                        return BadRequest("No file provided.");
                    }

                    await _customerService.UploadAvatarAsync(file, CustomerId);

                    return Ok("Avatar uploaded successfully!");
                }
                else
                {
                    return Forbid();
                }
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

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("")]
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
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerById(int customerId)
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

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("private")]
        public async Task<IActionResult> GetCustomerByIdPrivate([FromQuery] int customerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    var customer = await _customerService.GetCustomerByIdPrivate(customerId);
                    if (customer == null)
                    {
                        return NotFound("This customer does not exist");
                    }
                    return Ok(customer);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("change-status-booking-tour-guide/{bookingTourGuideRequestId}")]
        public async Task<IActionResult> ChangeStatusBookingTourGuide(int bookingTourGuideRequestId)
        {
            try
            {
                var result = await _customerService.ChangeStatusBookingTourGuideAsync(bookingTourGuideRequestId);
                return result ? Ok("Chuyến đi đánh dấu hoàn thành và thông báo đã được gửi.") : BadRequest("Không thể thay đổi trạng thái của chuyến đi");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("change-status-booking-tour/{bookingTourRequestId}")]
        public async Task<IActionResult> ChangeStatusBookingTour(int bookingTourRequestId)
        {
            try
            {
                var result = await _customerService.ChangeStatusBookingTourAsync(bookingTourRequestId);
                return result ? Ok("Chuyến đi đánh dấu hoàn thành và thông báo đã được gửi.") : BadRequest("Không thể thay đổi trạng thái của chuyến đi");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
