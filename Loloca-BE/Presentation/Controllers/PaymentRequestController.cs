using Loloca_BE.Business.Models.OrderView;
using Loloca_BE.Business.Models.PaymentRequestView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentRequestController : ControllerBase
    {
        private readonly IPaymentRequestService _paymentRequestService;

        public PaymentRequestController(IPaymentRequestService paymentRequestService)
        {
            _paymentRequestService = paymentRequestService;
        }

        [HttpPost("/api/v1/payment-request/deposit")]
        public async Task<ActionResult> DepositRequest([FromBody] DepositRequestView depositView)
        {
            try
            {
                if(depositView == null)
                {
                    return BadRequest("Request cannot null");
                }
                if(depositView.TransactionCode == null)
                {
                    return BadRequest("Transaction code cannot null");
                }
                if(depositView.Amount < 0 || depositView.Amount == null)
                {
                    return BadRequest("Invalid number of money");
                }
                if(depositView.AccountId == null)
                {
                    return BadRequest("Account Id cannot null");
                }
                await _paymentRequestService.SendDepositRequest(depositView);
                return Ok("Your request sent, our team will review within 7 working days");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/payment-request/deposit")]
        public async Task<ActionResult> GetAllDepositRequest([FromQuery] int? status)
        {
            try
            {
                var deposits = await _paymentRequestService.GetAllDepositRequest(status);
                return Ok(deposits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/payment-request/deposit/customer")]
        public async Task<ActionResult> GetDepositByCustomerId([FromQuery] int customerId, [FromQuery] int? status)
        {
            try
            {
                var deposits = await _paymentRequestService.GetDepositByCustomerId(customerId, status);
                return Ok(deposits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/payment-request/deposit/tourguide")]
        public async Task<ActionResult> GetDepositByTourGuideId([FromQuery] int tourGuideId, [FromQuery] int? status)
        {
            try
            {
                var deposits = await _paymentRequestService.GetDepositByTourGuideId(tourGuideId, status);
                return Ok(deposits);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }
    }
}
