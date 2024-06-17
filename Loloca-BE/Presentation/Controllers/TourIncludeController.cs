using AutoMapper;
using Loloca_BE.Business.Models.TourIncludeView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourIncludeController : Controller
    {
        private readonly ITourIncludeService _tourIncludeService;
        private readonly IMapper _mapper;

        public TourIncludeController(ITourIncludeService tourIncludeService, IMapper mapper)
        {
            _tourIncludeService = tourIncludeService;
            _mapper = mapper;
        }



        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost]
        public async Task<IActionResult> CreateTourInclude([FromBody] TourIncludeDTO includeDTO)
        {
            try
            {
                var includeId = await _tourIncludeService.CreateTourIncludeAsync(includeDTO);
                return Ok(new { IncludeId = includeId, Message = "Tour include created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPut("{includeId}")]
        public async Task<IActionResult> UpdateTourInclude(int includeId, [FromBody] TourIncludeDTO includeDTO)
        {
            try
            {
                includeDTO.IncludeId = includeId;
                await _tourIncludeService.UpdateTourIncludeAsync(includeDTO);
                return Ok("Tour include updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpDelete("{includeId}")]
        public async Task<IActionResult> DeleteTourInclude(int includeId)
        {
            try
            {
                await _tourIncludeService.DeleteTourIncludeAsync(includeId);
                return Ok("Tour include deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("{includeId}")]
        public async Task<IActionResult> GetTourIncludeByIdAsync(int includeId)
        {
            try
            {
                var includeDTO = await _tourIncludeService.GetTourIncludeByIdAsync(includeId);
                if (includeDTO == null)
                {
                    return NotFound();
                }
                return Ok(includeDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllTourIncludes()
        {
            try
            {
                var highlights = await _tourIncludeService.GetAllTourIncludeAsync();
                return Ok(highlights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
