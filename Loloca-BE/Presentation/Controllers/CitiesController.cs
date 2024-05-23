using Loloca_BE.Business.Models.CitiesView;
using Loloca_BE.Business.Services;
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
            var cities = await _citiesService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            var city = await _citiesService.GetCityByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityView cityView)
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
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityView cityView)
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
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var success = await _citiesService.DeleteCityAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return Ok("Xóa thành công");
        }
    }
}
