using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingTourGuideRequestController : Controller
    {
        private readonly IBookingTourGuideRequestService _bookingService;
        private readonly IAuthorizeService _authorizeService;

        public BookingTourGuideRequestController(IBookingTourGuideRequestService bookingService, IAuthorizeService authorizeService)
        {
            _bookingService = bookingService;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("create-booking-tourguide")]
        public async Task<IActionResult> CreateBookingTourGuideRequest([FromBody] BookingTourGuideRequestView model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Model is null.");
                }
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByCustomerId(model.CustomerId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    if (model.StartDate < DateTime.Now || model.EndDate < DateTime.Now || model.StartDate > model.EndDate)
                    {
                        return BadRequest("model.StartDate < DateTime.Now || model.EndDate < DateTime.Now || model.StartDate > model.EndDate");
                    }
                    var result = await _bookingService.AddBookingTourGuideRequestAsync(model);
                    return Ok("Tạo thành công");
                } else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("get-all-booking-tourguide")]
        public async Task<IActionResult> GetAllBookingTourGuideRequest()
        {
            try
            {
                var tourGuide = await _bookingService.GetAllBookingTourGuideRequestAsync();
                return Ok(tourGuide);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAllRoles")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingTourGuideRequestById(int id)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByCustomerId(1, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
                {
                    var tourGuide = await _bookingService.GetBookingTourGuideRequestByIdAsync(id);
                    if (tourGuide == null)
                    {
                        return NotFound();
                    }
                    return Ok(tourGuide);
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

        [Authorize(Policy = "RequireAdminOrCustomerRole")]
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<BookingTourGuideRequest>>> GetBookingTourGuideRequestByCustomerId(int customerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
                {
                    var requests = await _bookingService.GetBookingTourGuideRequestByCustomerId(customerId);
                    return Ok(requests);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireAdminOrTourGuideRole")]
        [HttpGet("tourguide/{tourGuideId}")]
        public async Task<ActionResult<IEnumerable<BookingTourGuideRequest>>> GetBookingTourGuideRequestByTourGuideId(int tourGuideId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourGuideId(tourGuideId, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
                {
                    var requests = await _bookingService.GetBookingTourGuideRequestByTourGuideId(tourGuideId);
                    return Ok(requests);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
