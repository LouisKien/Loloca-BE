using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
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
    }
}
