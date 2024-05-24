using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Services;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFeedbackAsync([FromForm] FeedbackModelView feedbackModel, [FromForm] List<IFormFile> images)
        {
            try
            {
                if (images == null || images.Count == 0)
                {
                    return BadRequest("No images uploaded.");
                }

                await _feedbackService.UploadFeedbackAsync(feedbackModel, images);
                return Ok("Feedback uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetFeedbackByCustomerIdAsync(int customerId)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackByCustomerIdAsync(customerId);
                if (feedback == null)
                {
                    return NotFound($"No feedback found for customer with ID {customerId}");
                }
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("tourguide/{tourGuideId}")]
        public async Task<IActionResult> GetFeedbackByTourGuideIdAsync(int tourGuideId)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackByTourGuideIdAsync(tourGuideId);
                if (feedback == null)
                {
                    return NotFound($"No feedback found for tour guide with ID {tourGuideId}");
                }
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacksAsync()
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{feedbackId}")]
        public async Task<IActionResult> GetFeedbackByIdAsync(int feedbackId)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackByIdAsync(feedbackId);
                if (feedback == null)
                {
                    return NotFound();
                }
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{feedbackId}/status")]
        public async Task<IActionResult> UpdateFeedbackStatusAsync(int feedbackId, [FromForm] bool newStatus)
        {
            try
            {
                var success = await _feedbackService.UpdateStatusAsync(feedbackId, newStatus);
                if (success)
                {
                    return Ok("Feedback status updated successfully.");
                }
                else
                {
                    return NotFound($"Feedback with ID {feedbackId} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
