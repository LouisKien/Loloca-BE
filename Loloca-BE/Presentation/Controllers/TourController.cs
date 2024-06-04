using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Models.TourView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly IAuthorizeService _authorizeService;
        private readonly IMemoryCache _cache;

        public TourController(ITourService tourService, IMemoryCache cache, IAuthorizeService authorizeService)
        {
            _tourService = tourService;
            _cache = cache;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("/uploadtour")]
        public async Task<IActionResult> UploadTourImages([FromForm] TourModelView tourModel, [FromForm] List<IFormFile> images)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourGuideId(tourModel.TourGuideId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    if (images == null || images.Count == 0)
                    {
                        return BadRequest("No images uploaded.");
                    }
                    await _tourService.UploadTourImageAsync(tourModel, images);
                    return Ok("Tour uploaded successfully.");
                }
                else {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPut("/updatetour/{tourId}")]
        public async Task<IActionResult> UpdateTour(int tourId, [FromForm] TourInfoView tourModel)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourId(tourId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    await _tourService.UpdateTourAsync(tourId, tourModel);
                    return Ok("Tour updated successfully.");
                }
                else
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

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpDelete("{tourId}")]
        public async Task<IActionResult> DeleteTour(int tourId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourId(tourId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    await _tourService.DeleteTourAsync(tourId);
                    return Ok("Tour deleted successfully.");
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpGet("random")]
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
                if (!tours.Any())
                {
                    return NotFound("No tour exist");
                }
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

        [AllowAnonymous]
        [HttpGet("city")]
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
                if (!tours.Any())
                {
                    return NotFound("No tour exist");
                }
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

        [AllowAnonymous]
        [HttpGet("tour-guide")]
        public async Task<IActionResult> GetRandomToursByTourGuide([FromQuery] int TourGuideId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var tours = await _tourService.GetToursByTourGuideAsync(TourGuideId, page, pageSize);
                if (!tours.Any())
                {
                    return NotFound("No tour exist");
                }
                var totalPage = await _tourService.GetTotalPageByTourGuide(TourGuideId, pageSize);
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

        private void AddSessionIdToCache(string sessionId)
        {
            var activeSessionIds = _cache.Get<List<string>>("ActiveSessions") ?? new List<string>();
            if (!activeSessionIds.Contains(sessionId))
            {
                activeSessionIds.Add(sessionId);
                _cache.Set("ActiveSessions", activeSessionIds, TimeSpan.FromMinutes(30));
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
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
