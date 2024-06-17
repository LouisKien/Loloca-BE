using AutoMapper;
using Loloca_BE.Business.Models.TourExcludeView;
using Loloca_BE.Business.Models.TourTypeView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourExcludeController : ControllerBase
    {
        private readonly ITourExcludeService _tourExcludeService;
        private readonly IMapper _mapper;

        public TourExcludeController(ITourExcludeService tourExcludeService, IMapper mapper)
        {
            _tourExcludeService = tourExcludeService;
            _mapper = mapper;
        }



        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost]
        public async Task<IActionResult> CreateTourExclude([FromBody] TourExcludeDTO excludeDTO)
        {
            try
            {
                var excludeId = await _tourExcludeService.CreateTourExcludeAsync(excludeDTO);
                return Ok(new { ExcludeId = excludeId, Message = "Tour exclude created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPut("{excludeId}")]
        public async Task<IActionResult> UpdateTourExclude(int excludeId, [FromBody] TourExcludeDTO excludeDTO)
        {
            try
            {
                excludeDTO.ExcludeId = excludeId;
                await _tourExcludeService.UpdateTourExcludeAsync(excludeDTO);
                return Ok("Tour exclude updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpDelete("{excludeId}")]
        public async Task<IActionResult> DeleteTourExclude(int excludeId)
        {
            try
            {
                await _tourExcludeService.DeleteTourExcludeAsync(excludeId);
                return Ok("Tour exclude deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("{excludeId}")]
        public async Task<IActionResult> GetTourExcludeByIdAsync(int excludeId)
        {
            try
            {
                var excludeDTO = await _tourExcludeService.GetTourExcludeByIdAsync(excludeId);
                if (excludeDTO == null)
                {
                    return NotFound();
                }
                return Ok(excludeDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllTourExcludes()
        {
            try
            {
                var highlights = await _tourExcludeService.GetAllTourExcludeAsync();
                return Ok(highlights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
