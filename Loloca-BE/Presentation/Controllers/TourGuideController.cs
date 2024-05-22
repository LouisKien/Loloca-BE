using Loloca_BE.Business.Models;
using Loloca_BE.Business.Services;
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
    }
}
