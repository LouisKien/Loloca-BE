using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICitiesService _citiesService;

        public CitiesController(ICitiesService citiesService)
        {
            _citiesService = citiesService;
        }

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

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityView cityView)
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


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityView cityView)
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
    }
}
