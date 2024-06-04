using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingTourGuideRequestController : Controller
    {
        private readonly IBookingTourGuideRequestService _bookingService;

        public BookingTourGuideRequestController(IBookingTourGuideRequestService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookingTourGuideRequest([FromBody] BookingTourGuideRequestView model)
        {
            if (model == null)
            {
                return BadRequest("Model is null.");
            }

            if(model.StartDate < DateTime.Now || model.EndDate < DateTime.Now || model.StartDate > model.EndDate)
            {
                return BadRequest("Cút");
            }
            try
            {
                var result = await _bookingService.AddBookingTourGuideRequestAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingTourGuideRequestById(int id)
        {
            try
            {
                var tourGuide = await _bookingService.GetBookingTourGuideRequestByIdAsync(id);
                if (tourGuide == null)
                {
                    return NotFound();
                }
                return Ok(tourGuide);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<BookingTourGuideRequest>>> GetBookingTourGuideRequestByCustomerId(int customerId)
        {
            try
            {
                var requests = await _bookingService.GetBookingTourGuideRequestByCustomerId(customerId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("tourguide/{tourGuideId}")]
        public async Task<ActionResult<IEnumerable<BookingTourGuideRequest>>> GetBookingTourGuideRequestByTourGuideId(int tourGuideId)
        {
            try
            {
                var requests = await _bookingService.GetBookingTourGuideRequestByTourGuideId(tourGuideId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
