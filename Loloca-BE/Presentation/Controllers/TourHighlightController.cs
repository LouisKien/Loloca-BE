using System;
using System.Threading.Tasks;
using AutoMapper;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Business.Models.TourHighlightView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourHighlightController : ControllerBase
    {
        private readonly ITourHighlightService _tourHighlightService;
        private readonly IMapper _mapper;

        public TourHighlightController(ITourHighlightService tourHighlightService, IMapper mapper)
        {
            _tourHighlightService = tourHighlightService;
            _mapper = mapper;
        }






        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost]
        public async Task<IActionResult> CreateTourHighlight([FromBody] TourHighlightDTO highlightDTO)
        {
            try
            {
                var highlightId = await _tourHighlightService.CreateTourHighlightAsync(highlightDTO);
                return Ok(new { HighlightId = highlightId, Message = "Tour highlight created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPut("{highlightId}")]
        public async Task<IActionResult> UpdateTourHighlight(int highlightId, [FromBody] TourHighlightDTO highlightDTO)
        {
            try
            {
                highlightDTO.HighlightId = highlightId;
                await _tourHighlightService.UpdateTourHighlightAsync(highlightDTO);
                return Ok("Tour highlight updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpDelete("{highlightId}")]
        public async Task<IActionResult> DeleteTourHighlight(int highlightId)
        {
            try
            {
                await _tourHighlightService.DeleteTourHighlightAsync(highlightId);
                return Ok("Tour highlight deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("{highlightId}")]
        public async Task<IActionResult> GetTourHighlightByIdAsync(int highlightId)
        {
            try
            {
                var highlightDTO = await _tourHighlightService.GetTourHighlightByIdAsync(highlightId);
                if (highlightDTO == null)
                {
                    return NotFound();
                }
                return Ok(highlightDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllTourHighlights()
        {
            try
            {
                var highlights = await _tourHighlightService.GetAllTourHighlightsAsync();
                return Ok(highlights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
