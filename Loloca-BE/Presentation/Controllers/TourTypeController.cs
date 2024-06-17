using AutoMapper;
using Loloca_BE.Business.Models.TourHighlightView;
using Loloca_BE.Business.Models.TourTypeView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourTypeController : ControllerBase
    {
        private readonly ITourTypeService _tourTypeService;
        private readonly IMapper _mapper;

        public TourTypeController(ITourTypeService tourTypeService, IMapper mapper)
        {
            _tourTypeService = tourTypeService;
            _mapper = mapper;
        }



        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPost]
        public async Task<IActionResult> CreateTourType([FromBody] TourTypeDTO typeDTO)
        {
            try
            {
                var typeId = await _tourTypeService.CreateTourTypeAsync(typeDTO);
                return Ok(new { TypeId = typeId, Message = "Tour type created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpPut("{typeId}")]
        public async Task<IActionResult> UpdateTourType(int typeId, [FromBody] TourTypeDTO typeDTO)
        {
            try
            {
                typeDTO.TypeId = typeId;
                await _tourTypeService.UpdateTourTypeAsync(typeDTO);
                return Ok("Tour type updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireTourGuideRole")]
        [HttpDelete("{typeId}")]
        public async Task<IActionResult> DeleteTourType(int typeId)
        {
            try
            {
                await _tourTypeService.DeleteTourTypeAsync(typeId);
                return Ok("Tour type deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("{typeId}")]
        public async Task<IActionResult> GetTourTypeByIdAsync(int typeId)
        {
            try
            {
                var typeDTO = await _tourTypeService.GetTourTypeByIdAsync(typeId);
                if (typeDTO == null)
                {
                    return NotFound();
                }
                return Ok(typeDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllTourType()
        {
            try
            {
                var highlights = await _tourTypeService.GetAllTourTypeAsync();
                return Ok(highlights);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
