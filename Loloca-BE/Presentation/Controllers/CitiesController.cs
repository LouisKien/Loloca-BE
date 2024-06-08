using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICitiesService _citiesService;
        private readonly IAuthorizeService _authorizeService;

        public CitiesController(ICitiesService citiesService, IAuthorizeService authorizeService)
        {
            _citiesService = citiesService;
            _authorizeService = authorizeService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            try
            {
                var cities = await _citiesService.GetAllCitiesAsync();
                return Ok(cities);
            } catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            try
            {
                var city = await _citiesService.GetCityByIdAsync(id);
                if (city == null)
                {
                    return NotFound();
                }
                return Ok(city);
            } catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CreateCity cityView)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdCity = await _citiesService.AddCityAsync(cityView);
                if (createdCity == null)
                {
                    return BadRequest("Failed to create city.");
                }

                return Ok(createdCity); // Return the created city with HTTP status 200 (OK)
            } catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] UpdateCityView cityView)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedCity = await _citiesService.UpdateCityAsync(id, cityView);
                if (updatedCity == null)
                {
                    return NotFound();
                }

                return Ok("Cập nhật thành công");
            } catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            try
            {
                var success = await _citiesService.DeleteCityAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return Ok("Xóa thành công");
            } catch(Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("uploadcitybanner")]
        public async Task<IActionResult> UploadCityBanner([FromForm] int CityId, [FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest("No files uploaded.");
                }

                foreach (var file in files)
                {
                    if (!IsImage(file))
                    {
                        return BadRequest("Only image files are allowed.");
                    }
                }

                foreach (var file in files)
                {
                    await _citiesService.UploadCityBannerAsync(file, CityId);
                }

                return Ok("City banner uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("uploadcitythumbnail")]
        public async Task<IActionResult> UploadCityThumbnail([FromForm] int CityId, [FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest("No files uploaded.");
                }

                foreach (var file in files)
                {
                    if (!IsImage(file))
                    {
                        return BadRequest("Only image files are allowed.");
                    }
                }

                foreach (var file in files)
                {
                    await _citiesService.UploadCityThumbnailAsync(file, CityId);
                }

                return Ok("City thumbnail uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool IsImage(IFormFile file)
        {
            return file.ContentType.StartsWith("image/");
        }


    }
}
