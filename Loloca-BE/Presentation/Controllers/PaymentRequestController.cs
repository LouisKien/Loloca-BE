﻿using Loloca_BE.Business.Models.OrderView;
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

        // ----------------------------------------- DEPOSIT --------------------------------------------
        [HttpPost("/api/v1/payment-request/deposit")]
        public async Task<ActionResult> DepositRequest([FromBody] DepositRequestView depositView)
        {
            try
            {
                if (depositView == null)
                {
                    return BadRequest("Request cannot be null");
                }
                if (depositView.TransactionCode == null)
                {
                    return BadRequest("Transaction code cannot be null");
                }
                if (depositView.Amount < 0)
                {
                    return BadRequest("Invalid amount of money");
                }
                if (depositView.AccountId == 0)
                {
                    return BadRequest("Account Id cannot be null");
                }
                await _paymentRequestService.SendDepositRequest(depositView);
                return Ok("Your request has been sent, our team will review it within 7 working days");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
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

        [HttpGet("/api/v1/payment-request/deposit/{PaymentRequestId}")]
        public async Task<ActionResult> GetAllDepositRequest(int PaymentRequestId)
        {
            try
            {
                var deposit = await _paymentRequestService.GetDepositById(PaymentRequestId);
                if (deposit == null)
                {
                    return NotFound($"Does not find any request with id {PaymentRequestId}");
                }
                else
                {
                    return Ok(deposit);
                }
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

        [HttpPut("/api/v1/payment-request/deposit")]
        public async Task<ActionResult> UpdateStatusDeposit([FromQuery] int paymentRequestId, [FromQuery] int status)
        {
            try
            {
                await _paymentRequestService.UpdateStatusDepositAsync(paymentRequestId, status);
                if (status == 1)
                {
                    return Ok("Accept request successfully");
                }
                else if (status == 2)
                {
                    return Ok("Reject request successfully");
                }
                else
                {
                    return BadRequest("Invalid status value");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Không tìm thấy yêu cầu nạp tiền"))
                {
                    return BadRequest($"Deposit request with id {paymentRequestId} does not exist");
                }
                else if (ex.Message.Contains("Loại giao dịch không phù hợp"))
                {
                    return BadRequest("Not correct type of transaction");
                }
                else if (ex.Message.Contains("Yêu cầu đã được xử lý trước đó"))
                {
                    return BadRequest("Cannot approve this request: This request have already approved before");
                }
                else if (ex.Message.Contains("Admin role can't approve their request"))
                {
                    return BadRequest("Admin role can't approve their request");
                }
                else
                {
                    return StatusCode(500, $"Internal Server Error: {ex.Message}");
                }
            }
        }


        // ----------------------------------------- WITHDRAWAL --------------------------------------------
        [HttpPost("/api/v1/payment-request/withdrawal")]
        public async Task<ActionResult> WithdrawalRequest([FromBody] WithdrawalView withdrawalView)
        {
            try
            {
                if (withdrawalView == null)
                {
                    return BadRequest("Request cannot null");
                }
                if (withdrawalView.BankAccount == null)
                {
                    return BadRequest("Bank account cannot null");
                }
                if (withdrawalView.Bank == null)
                {
                    return BadRequest("Bank cannot null");
                }
                if (withdrawalView.Amount < 0 || withdrawalView.Amount == null)
                {
                    return BadRequest("Invalid number of money");
                }
                if (withdrawalView.AccountId == null)
                {
                    return BadRequest("Account Id cannot null");
                }
                var status = await _paymentRequestService.SendWithdrawalRequest(withdrawalView);
                if (status == 1)
                {
                    return Ok("Your request sent, our team will review within 7 working days");
                }
                else if (status == -1)
                {
                    return BadRequest("Admin role can't send request");
                } else if (status == -2)
                {
                    return BadRequest("Out of balance in Loloca wallet");
                }
                return BadRequest("Send request failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/payment-request/withdrawal")]
        public async Task<ActionResult> GetAllWithdrawalRequest([FromQuery] int? status)
        {
            try
            {
                var withdrawals = await _paymentRequestService.GetAllWithdrawalRequest(status);
                return Ok(withdrawals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/payment-request/withdrawal/{PaymentRequestId}")]
        public async Task<ActionResult> GetAllWithdrawalRequest(int PaymentRequestId)
        {
            try
            {
                var withdrawal = await _paymentRequestService.GetAllWithdrawalById(PaymentRequestId);
                if (withdrawal == null)
                {
                    return NotFound($"Does not find any request with id {PaymentRequestId}");
                }
                else
                {
                    return Ok(withdrawal);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/payment-request/withdrawal/customer")]
        public async Task<ActionResult> GetWithdrawalByCustomerId([FromQuery] int customerId, [FromQuery] int? status)
        {
            try
            {
                var withdrawals = await _paymentRequestService.GetAllWithdrawalByCustomerId(customerId, status);
                return Ok(withdrawals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/payment-request/withdrawal/tourguide")]
        public async Task<ActionResult> GetWithdrawalByTourGuideId([FromQuery] int tourGuideId, [FromQuery] int? status)
        {
            try
            {
                var withdrawals = await _paymentRequestService.GetAllWithdrawalByTourGuideId(tourGuideId, status);
                return Ok(withdrawals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("/api/v1/payment-request/withdrawal")]
        public async Task<ActionResult> UpdateStatusWithdrawal([FromQuery] int paymentRequestId)
        {
            try
            {
                await _paymentRequestService.UpdateStatusWithdrawalAsync(paymentRequestId);
                return Ok("Withdrawal request successfully accepted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }






    }
}