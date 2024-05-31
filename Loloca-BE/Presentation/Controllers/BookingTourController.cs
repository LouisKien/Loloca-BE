using Loloca_BE.Business.Models.BookingTourGuideRequestModelView;
using Loloca_BE.Business.Models.BookingTourRequestModelView;
using Loloca_BE.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingTourController : ControllerBase
    {
        private readonly IBookingTourRequestService _bookingTourRequestService;

        public BookingTourController(IBookingTourRequestService bookingTourRequestService)
        {
            _bookingTourRequestService = bookingTourRequestService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookingTourRequest([FromBody] BookingTourRequestView model)
        {
            if (model == null)
            {
                return BadRequest("Model is null.");
            }

            try
            {
                var result = await _bookingTourRequestService.AddBookingTourRequestAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingTourRequestById(int id)
        {
            try
            {
                var tourGuide = await _bookingTourRequestService.GetBookingTourRequestByIdAsync(id);
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
