using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly IMemoryCache _cache;

        public TourController(ITourService tourService, IMemoryCache cache)
        {
            _tourService = tourService;
            _cache = cache;
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

        [HttpPut("/accept-tour-change-status/{tourId}")]
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

        [HttpGet("{tourId}")]
        public async Task<IActionResult> GetTourByIdAsync(int tourId)
        {
            try
            {
                var tour = await _tourService.GetTourByIdAsync(tourId);
                if (tour == null)
                {
                    return NotFound();
                }
                return Ok(tour);
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

                if (HttpContext.Session.GetString("SessionId") == null)
                {
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("SessionId", sessionId);
                    AddSessionIdToCache(sessionId);
                }
                else
                {
                    sessionId = HttpContext.Session.GetString("SessionId");
                }
                var tours = await _tourService.GetRandomToursAsync(sessionId, page, pageSize);
                var totalPage = await _tourService.GetTotalPage(pageSize, null, sessionId);
                if (page > totalPage)
                {
                    return NotFound("This page does not exist.");
                }
                return Ok(new { tours, totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/tour/city")]
        public async Task<IActionResult> GetRandomToursInCity([FromQuery] int CityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                string sessionId;

                if (HttpContext.Session.GetString("SessionId") == null)
                {
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("SessionId", sessionId);
                    AddSessionIdToCache(sessionId);
                }
                else
                {
                    sessionId = HttpContext.Session.GetString("SessionId");
                }
                var tours = await _tourService.GetRandomToursInCityAsync(sessionId, CityId, page, pageSize);
                var totalPage = await _tourService.GetTotalPage(pageSize, CityId, sessionId);
                if (page > totalPage)
                {
                    return NotFound("This page does not exist.");
                }
                return Ok(new { tours, totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/tour/tourGuide")]
        public async Task<IActionResult> GetRandomToursByTourGuide([FromQuery] int TourGuideId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var totalPage = await _tourService.GetTotalPageByTourGuide(TourGuideId, pageSize);
                if (page > totalPage)
                {
                    return NotFound("This page does not exist.");
                }
                var tours = await _tourService.GetToursByTourGuideAsync(TourGuideId, page, pageSize);
                return Ok(new { tours, totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        private void AddSessionIdToCache(string sessionId)
        {
            var activeSessionIds = _cache.Get<List<string>>("ActiveSessions") ?? new List<string>();
            if (!activeSessionIds.Contains(sessionId))
            {
                activeSessionIds.Add(sessionId);
                _cache.Set("ActiveSessions", activeSessionIds, TimeSpan.FromMinutes(30));
            }
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetToursByStatusAsync(int status)
        {
            try
            {
                var tours = await _tourService.GetToursByStatusAsync(status);
                if (tours == null || tours.Count == 0)
                {
                    return NotFound("No tours found with the given status.");
                }
                return Ok(tours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
