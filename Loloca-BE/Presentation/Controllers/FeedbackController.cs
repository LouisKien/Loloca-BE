using Loloca_BE.Business.Models.FeedbackView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IAuthorizeService _authorizeService;

        public FeedbackController(IFeedbackService feedbackService, IAuthorizeService authorizeService)
        {
            _feedbackService = feedbackService;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFeedbackAsync([FromForm] FeedbackModelView feedbackModel, [FromForm] List<IFormFile> images)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByCustomerId(feedbackModel.CustomerId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    if (images == null || images.Count == 0)
                    {
                        return BadRequest("No images uploaded.");
                    }

                    await _feedbackService.UploadFeedbackAsync(feedbackModel, images);
                    return Ok("Feedback uploaded successfully.");
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize("RequireAdminOrCustomerRole")]
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetFeedbackByCustomerIdAsync(int customerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
                {
                    var feedback = await _feedbackService.GetFeedbackByCustomerIdAsync(customerId);
                    if (feedback == null)
                    {
                        return NotFound($"No feedback found for customer with ID {customerId}");
                    }
                    return Ok(feedback);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
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

        [Authorize(Policy = "RequireAdminRole")]
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

        [AllowAnonymous]
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

        [Authorize(Policy = "RequireAdminOrCustomerRole")]
        [HttpPut("{feedbackId}/status")]
        public async Task<IActionResult> UpdateFeedbackStatusAsync(int feedbackId, [FromForm] bool newStatus)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByFeedbackId(feedbackId, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
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
                } else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
