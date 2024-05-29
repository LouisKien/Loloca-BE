using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        [HttpPost("/uploadtour")]
        public async Task<IActionResult> UploadTourImages([FromForm] TourModelView tourModel, [FromForm] List<IFormFile> images)
        {
            try
            {
                if (images == null || images.Count == 0)
                {
                    return BadRequest("No images uploaded.");
                }

                await _tourService.UploadTourImageAsync(tourModel, images);
                return Ok("Tour uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("/updatetour/{tourId}")]
        public async Task<IActionResult> UpdateTour(int tourId, [FromForm] TourInfoView tourModel)
        {
            try
            {
                await _tourService.UpdateTourAsync(tourId, tourModel);
                return Ok("Tour updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("/updatetour-status/{tourId}")]
        public async Task<IActionResult> UpdateTourStatus(int tourId, [FromForm] TourStatusView tourModel)
        {
            try
            {
                await _tourService.UpdateTourStatusAsync(tourId, tourModel);
                return Ok("Status updated successfully");
            }catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }
    }

}
