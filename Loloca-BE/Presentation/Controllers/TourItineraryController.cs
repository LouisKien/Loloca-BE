using AutoMapper;
using Loloca_BE.Business.Models.TourIncludeView;
using Loloca_BE.Business.Models.TourItineraryView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourItineraryController : Controller
    {
        private readonly ITourItineraryService _tourItineraryService;
        private readonly IMapper _mapper;

        public TourItineraryController(ITourItineraryService tourItineraryService, IMapper mapper)
        {
            _tourItineraryService = tourItineraryService;
            _mapper = mapper;
        }



        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost]
        public async Task<IActionResult> CreateTourItinerary([FromBody] TourItineraryDTO itineraryDTO)
        {
            try
            {
                var itineraryId = await _tourItineraryService.CreateTourItineraryAsync(itineraryDTO);
                return Ok(new { ItineraryId = itineraryId, Message = "Tour itinerary created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPut("{itineraryId}")]
        public async Task<IActionResult> UpdateTourItinerary(int itineraryId, [FromBody] TourItineraryDTO itineraryDTO)
        {
            try
            {
                itineraryDTO.ItineraryId = itineraryId;
                await _tourItineraryService.UpdateTourItineraryAsync(itineraryDTO);
                return Ok("Tour itinerary updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpDelete("{itineraryId}")]
        public async Task<IActionResult> DeleteTourItinerary(int itineraryId)
        {
            try
            {
                await _tourItineraryService.DeleteTourItineraryAsync(itineraryId);
                return Ok("Tour itinerary deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("{itineraryId}")]
        public async Task<IActionResult> GetTourItineraryByIdAsync(int itineraryId)
        {
            try
            {
                var itineraryDTO = await _tourItineraryService.GetTourItineraryByIdAsync(itineraryId);
                if (itineraryDTO == null)
                {
                    return NotFound();
                }
                return Ok(itineraryDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllTourItinerary()
        {
            try
            {
                var highlights = await _tourItineraryService.GetAllTourItineraryAsync();
                return Ok(highlights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
