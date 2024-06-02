using Loloca_BE.Business.Models;
using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourGuideController : ControllerBase
    {
        private readonly ITourGuideService _tourGuideService;
        private readonly IMemoryCache _cache;

        public TourGuideController(ITourGuideService tourGuideService, IMemoryCache cache)
        {
            _tourGuideService = tourGuideService;
            _cache = cache;
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/tourguide/update-avatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] IFormFile file, [FromForm] int TourGuideId)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest("No file provided.");
                }

                await _tourGuideService.UploadAvatarAsync(file, TourGuideId);

                return Ok("Avatar uploaded successfully!");
            }
            catch (InvalidDataException ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("/api/v1/tourguide/update-cover")]
        public async Task<IActionResult> UpdateCover([FromForm] IFormFile file, [FromForm] int TourGuideId)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest("No file provided.");
                }

                await _tourGuideService.UploadCoverAsync(file, TourGuideId);

                return Ok("Cover uploaded successfully!");
            }
            catch (InvalidDataException ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("update-info")]
        public async Task<IActionResult> UpdateTourGuideInfo(int tourGuideId, [FromBody] UpdateProfileTourGuide model)
        {
            try
            {
                await _tourGuideService.UpdateTourGuideInfo(tourGuideId, model);
                return Ok("Cập nhật thông tin thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(int accountId, [FromBody] ChangePasswordTourGuide model)
        {
            try
            {
                var success = await _tourGuideService.ChangeTourGuidePassword(accountId, model);
                if (success)
                {
                    return Ok("Đổi mật khẩu thành công");
                }
                else
                {
                    return BadRequest("Đổi mật khẩu không thành công");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/tourguide/info/{tourGuideId}")]
        public async Task<IActionResult> GetTourGuideInfo(int tourGuideId)
        {
            try
            {
                var tourGuideInfo = await _tourGuideService.GetTourGuideInfoAsync(tourGuideId);
                return Ok(tourGuideInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/tourguide")]
        public async Task<IActionResult> GetRandomTourGuides([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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

                var tourGuides = await _tourGuideService.GetRandomTourGuidesAsync(sessionId, page, pageSize);
                var totalPage = await _tourGuideService.GetTotalPage(pageSize, null, sessionId);
                if (page > totalPage)
                {
                    return NotFound("This page does not exist.");
                }
                return Ok(new { tourGuides, totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
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

        [HttpGet("/api/v1/tourguide/city")]
        public async Task<IActionResult> GetRandomTourGuidesInCity([FromQuery] int CityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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
                var tourGuides = await _tourGuideService.GetRandomTourGuidesInCityAsync(sessionId, CityId, page, pageSize);
                var totalPage = await _tourGuideService.GetTotalPage(pageSize, CityId, sessionId);
                if (page > totalPage)
                {
                    return NotFound("This page does not exist.");
                }
                return Ok(new { tourGuides, totalPage });
            } catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("accept-booking-tourguide-request")]
        public async Task<IActionResult> AcceptBookingRequest(int bookingRequestId)
        {
            try
            {
                var result = await _tourGuideService.AcceptRequestBookingTourGuideRequest(bookingRequestId);
                if (result)
                {
                    return Ok("Booking request accepted successfully.");
                }
                else
                {
                    return BadRequest("Failed to accept booking request.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("reject-booking-tourguide-request")]
        public async Task<IActionResult> RejectBookingRequest(int bookingRequestId)
        {
            try
            {
                var result = await _tourGuideService.RejectRequestBookingTourGuideRequest(bookingRequestId);
                if (result)
                {
                    return Ok("Booking request rejected successfully.");
                }
                else
                {
                    return BadRequest("Failed to rejected booking request.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





        [HttpPost("accept-booking-tour-request")]
        public async Task<IActionResult> AcceptBookingTourRequest(int bookingRequestId)
        {
            try
            {
                var result = await _tourGuideService.AcceptRequestBookingTourRequest(bookingRequestId);
                if (result)
                {
                    return Ok("Booking request accepted successfully.");
                }
                else
                {
                    return BadRequest("Failed to accept booking request.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("reject-booking-tour-request")]
        public async Task<IActionResult> RejectBookingTourRequest(int bookingRequestId)
        {
            try
            {
                var result = await _tourGuideService.RejectRequestBookingTourRequest(bookingRequestId);
                if (result)
                {
                    return Ok("Booking request rejected successfully.");
                }
                else
                {
                    return BadRequest("Failed to rejected booking request.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
