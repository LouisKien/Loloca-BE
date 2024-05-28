using Loloca_BE.Business.Models;
using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Business.Services;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourGuideController : ControllerBase
    {
        private readonly ITourGuideService _tourGuideService;

        public TourGuideController(ITourGuideService tourGuideService)
        {
            _tourGuideService = tourGuideService;
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
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return StatusCode(500, $"Lỗi: {ex.Message}");
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
                return StatusCode(500, $"Lỗi: {ex.Message}");
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/api/v1/tourguide")]
        public async Task<IActionResult> GetRandomTourGuides([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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

            var lastFetchIdString = HttpContext.Session.GetString("LastFetchIdCity");
            if (lastFetchIdString != null && int.TryParse(lastFetchIdString, out var parsedLastFetchId))
            {
                lastFetchId = parsedLastFetchId;
            }
            else
            {
                lastFetchId = null;
            }

            var totalPage = await _tourGuideService.GetTotalPage(pageSize, null);
            if (page > totalPage)
            {
                return NotFound("This page does not exist.");
            }
            var tourGuides = await _tourGuideService.GetRandomTourGuidesAsync(sessionId, page, pageSize, lastFetchId);
            var lastTourGuideAddedId = await _tourGuideService.GetLastTourGuideAddedIdAsync();
            HttpContext.Session.SetString("LastFetchIdCity", lastTourGuideAddedId.ToString());
            return Ok(new { tourGuides, totalPage});
        }

        [HttpGet("/api/v1/tourguide/city")]
        public async Task<IActionResult> GetRandomTourGuidesInCity([FromQuery] int CityId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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

            var lastFetchIdString = HttpContext.Session.GetString("LastFetchId");
            if (lastFetchIdString != null && int.TryParse(lastFetchIdString, out var parsedLastFetchId))
            {
                lastFetchId = parsedLastFetchId;
            }
            else
            {
                lastFetchId = null;
            }
            var totalPage = await _tourGuideService.GetTotalPage(pageSize, CityId);
            if(page > totalPage)
            {
                return NotFound("This page does not exist.");
            }
            var tourGuides = await _tourGuideService.GetRandomTourGuidesInCityAsync(sessionId, CityId, page, pageSize, lastFetchId);
            var lastTourGuideAddedIdInCity = await _tourGuideService.GetLastTourGuideAddedIdInCityAsync(CityId);
            HttpContext.Session.SetString("LastFetchId", lastTourGuideAddedIdInCity.ToString());
            return Ok(new { tourGuides, totalPage });
        }
    }
}
