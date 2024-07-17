using Loloca_BE.Business.Models;
using Loloca_BE.Business.Models.TourGuideView;
using Loloca_BE.Business.Services.Implements;
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
        private readonly IAuthorizeService _authorizeService;
        private readonly IMemoryCache _cache;

        public TourGuideController(ITourGuideService tourGuideService, IMemoryCache cache, IAuthorizeService authorizeService)
        {
            _tourGuideService = tourGuideService;
            _cache = cache;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [AllowAnonymous]
        [HttpPost("update-avatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] List<IFormFile> files, [FromForm] int TourGuideId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourGuideId(TourGuideId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    if (!files.Any())
                    {
                        return BadRequest("No file provided.");
                    }
                    var file = files.FirstOrDefault();
                    if (file == null)
                    {
                        return BadRequest("No file provided.");
                    }

                    await _tourGuideService.UploadAvatarAsync(file, TourGuideId);

                    return Ok("Avatar uploaded successfully!");
                } else
                {
                    return Forbid();
                } 
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

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("update-cover")]
        public async Task<IActionResult> UpdateCover([FromForm] List<IFormFile> files, [FromForm] int TourGuideId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourGuideId(TourGuideId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    if (!files.Any())
                    {
                        return BadRequest("No file provided.");
                    }
                    var file = files.FirstOrDefault();
                    if (file == null)
                    {
                        return BadRequest("No file provided.");
                    }

                    await _tourGuideService.UploadCoverAsync(file, TourGuideId);

                    return Ok("Cover uploaded successfully!");
                }
                else
                {
                    return Forbid();
                }
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

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("update-info/{tourGuideId}")]
        public async Task<IActionResult> UpdateTourGuideInfo([FromRoute] int tourGuideId, [FromBody] UpdateProfileTourGuide model)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourGuideId(tourGuideId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    await _tourGuideService.UpdateTourGuideInfo(tourGuideId, model);
                    return Ok("Cập nhật thông tin thành công");
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


        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("change-password/{accountId}")]
        public async Task<IActionResult> ChangePassword([FromRoute] int accountId, [FromBody] ChangePasswordTourGuide model)
        {
            try
            {
                var accountIdJwt = User.FindFirst("AccountId")?.Value;
                if (accountIdJwt == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByAccountId(accountId, int.Parse(accountIdJwt));
                if (checkAuthorize.isUser)
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


        [AllowAnonymous]
        [HttpGet("info/{tourGuideId}")]
        public async Task<IActionResult> GetTourGuideInfo(int tourGuideId)
        {
            try
            {
                var tourGuideInfo = await _tourGuideService.GetTourGuideInfoAsync(tourGuideId);
                if(tourGuideInfo.AccountStatus != 1)
                {
                    return BadRequest("This tour guide is not available now");
                }
                return Ok(tourGuideInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpGet("private-info/{tourGuideId}")]
        public async Task<IActionResult> GetTourGuidePrivateInfo(int tourGuideId)
        {
            try
            {
                var accountIdJwt = User.FindFirst("AccountId")?.Value;
                if (accountIdJwt == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourGuideId(tourGuideId, int.Parse(accountIdJwt));
                if (checkAuthorize.isUser)
                {
                    var tourGuideInfo = await _tourGuideService.GetPrivateTourGuideInfoAsync(tourGuideId);
                    return Ok(tourGuideInfo);
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

        [AllowAnonymous]
        [HttpGet("get-random-tourguide")]
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

        [AllowAnonymous]
        [HttpGet("get-all-tourguides")]
        public async Task<IActionResult> GetAllTourGuides([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var tourGuides = await _tourGuideService.GetTourGuidesAsync(page, pageSize);
                var totalPage = await _tourGuideService.GetTotalPage(pageSize);
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

        [AllowAnonymous]
        [HttpGet("get-random-tourguide-in-city")]
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

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("accept-booking-tourguide-request/{bookingRequestId}")]
        public async Task<IActionResult> AcceptBookingRequest([FromRoute] int bookingRequestId)
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



        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("reject-booking-tourguide-request/{bookingRequestId}")]
        public async Task<IActionResult> RejectBookingRequest([FromRoute] int bookingRequestId)
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
                    return BadRequest("Failed to reject booking request.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("accept-booking-tour-request/{bookingRequestId}")]
        public async Task<IActionResult> AcceptBookingTourRequest([FromRoute] int bookingRequestId)
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


        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost("reject-booking-tour-request/{bookingRequestId}")]
        public async Task<IActionResult> RejectBookingTourRequest([FromRoute] int bookingRequestId)
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
                    return BadRequest("Failed to reject booking request.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("{cityId}")]
        public async Task<IActionResult> GetTourGuidesByCityId(int cityId)
        {
            var tourGuides = await _tourGuideService.GetTourGuidesByCityId(cityId);
            return Ok(tourGuides);
        }


    }
}
