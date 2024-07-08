using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Models.BookingTourRequestModelView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingTourController : ControllerBase
    {
        private readonly IBookingTourRequestService _bookingTourRequestService;
        private readonly IAuthorizeService _authorizeService;

        public BookingTourController(IBookingTourRequestService bookingTourRequestService, IAuthorizeService authorizeService)
        {
            _bookingTourRequestService = bookingTourRequestService;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("create-booking-tour-request")]
        public async Task<IActionResult> CreateBookingTourRequest([FromBody] BookingTourRequestView model)
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
                if(checkAuthorize.isUser)
                {

                    if(model.StartDate < DateTime.Now || model.EndDate < DateTime.Now || model.StartDate > model.EndDate)
                    {
                        return BadRequest();
                    }
                    var result = await _bookingTourRequestService.AddBookingTourRequestAsync(model);

                    // Fetch the tour details to get the tour name
                    var tour = await _bookingTourRequestService.GetTourByIdAsync(model.TourId);
                    if (tour == null)
                    {
                        return NotFound("Tour not found.");
                    }

                    var response = new
                    {
                        result.BookingTourRequestId,
                        result.TourId,
                        TourName = tour.Name,
                        result.CustomerId,
                        result.RequestDate,
                        result.RequestTimeOut,
                        result.StartDate,
                        result.EndDate,
                        result.TotalPrice,
                        result.Note,
                        result.Status,
                        result.NumOfChild,
                        result.NumOfAdult
                    };

                    return Ok(response);
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
        [HttpGet("get-all-booking-tour-request")]
        public async Task<IActionResult> GetAllBookingTourRequest()
        {
            try
            {
                var tourGuide = await _bookingTourRequestService.GetAllBookingTourRequestAsync();
                return Ok(tourGuide);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAllRoles")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingTourRequestById(int id)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByBookingTourRequestId(id, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
                {
                    var tourGuide = await _bookingTourRequestService.GetBookingTourRequestByIdAsync(id);
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
        public async Task<ActionResult<IEnumerable<BookingTourRequest>>> GetBookingTourRequestByCustomerId(int customerId)
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
                    var requests = await _bookingTourRequestService.GetBookingTourRequestByCustomerId(customerId);
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
        public async Task<ActionResult<IEnumerable<BookingTourRequest>>> GetBookingTourRequestByTourGuideId(int tourGuideId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourGuideId(tourGuideId, int.Parse(accountId));
                if (checkAuthorize.isUser == false || checkAuthorize.isAdmin == false)
                {
                    var requests = await _bookingTourRequestService.GetBookingTourRequestByTourGuideId(tourGuideId);
                    return Ok(requests);
                }
                else { return Forbid(); }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
