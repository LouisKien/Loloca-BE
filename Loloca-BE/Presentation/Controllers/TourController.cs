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

        [HttpDelete("{tourId}")]
        public async Task<IActionResult> DeleteTour(int tourId)
        {
            try
            {
                await _tourService.DeleteTourAsync(tourId);
                return Ok("Tour deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/tour")]
        public async Task<IActionResult> GetRandomTours([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                string sessionId;
                int? lastFetchId = null;

                if (HttpContext.Session.GetString("SessionId") == null)
                {
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("SessionId", sessionId);
                }
                else
                {
                    sessionId = HttpContext.Session.GetString("SessionId");
                }
                var totalPage = await _tourService.GetTotalPage(pageSize);
                if (page > totalPage)
                {
                    return NotFound("This page does not exist.");
                }
                var tours = await _tourService.GetRandomToursAsync(sessionId, page, pageSize, lastFetchId);
                return Ok(new { tours, totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }
    }

}
