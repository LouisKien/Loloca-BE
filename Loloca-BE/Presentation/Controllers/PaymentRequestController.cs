using Loloca_BE.Business.Models.OrderView;
using Loloca_BE.Business.Models.PaymentRequestView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentRequestController : ControllerBase
    {
        private readonly IPaymentRequestService _paymentRequestService;
        private readonly IAuthorizeService _authorizeService;

        public PaymentRequestController(IPaymentRequestService paymentRequestService, IAuthorizeService authorizeService)
        {
            _paymentRequestService = paymentRequestService;
            _authorizeService = authorizeService;
        }

        // ----------------------------------------- DEPOSIT --------------------------------------------
        [Authorize(Policy = "RequireTourGuideOrCustomerRole")]
        [HttpPost("create-deposit")]
        public async Task<ActionResult> DepositRequest([FromBody] DepositRequestView depositView)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByAccountId(depositView.AccountId, int.Parse(accountId));
                if (checkAuthorize.isUser)
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
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("get-all-deposit")]
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

        [Authorize(Policy = "RequireAllRoles")]
        [HttpGet("deposit/{PaymentRequestId}")]
        public async Task<ActionResult> GetDepositRequest(int PaymentRequestId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByPaymentRequestId(PaymentRequestId, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
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
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminOrCustomerRole")]
        [HttpGet("deposit/customer")]
        public async Task<ActionResult> GetDepositByCustomerId([FromQuery] int customerId, [FromQuery] int? status)
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
                    var deposits = await _paymentRequestService.GetDepositByCustomerId(customerId, status);
                    return Ok(deposits);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminOrTourGuideRole")]
        [HttpGet("deposit/tourguide")]
        public async Task<ActionResult> GetDepositByTourGuideId([FromQuery] int tourGuideId, [FromQuery] int? status)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourGuideId(tourGuideId, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
                {
                    var deposits = await _paymentRequestService.GetDepositByTourGuideId(tourGuideId, status);
                    return Ok(deposits);
                } else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("update-deposit")]
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
        [Authorize(Policy = "RequireTourGuideOrCustomerRole")]
        [HttpPost("withdrawal")]
        public async Task<ActionResult> WithdrawalRequest([FromBody] WithdrawalView withdrawalView)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByAccountId(withdrawalView.AccountId, int.Parse(accountId));
                if (checkAuthorize.isUser)
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
                    }
                    else if (status == -2)
                    {
                        return BadRequest("Out of balance in Loloca wallet");
                    }
                    return BadRequest("Send request failed");
                }
                else
                {
                    return Forbid();
                }
                    
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("create-withdrawal")]
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

        [Authorize(Policy = "RequireAllRoles")]
        [HttpGet("withdrawal/{PaymentRequestId}")]
        public async Task<ActionResult> GetWithdrawalRequest(int PaymentRequestId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByPaymentRequestId(PaymentRequestId, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
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
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminOrCustomerRole")]
        [HttpGet("withdrawal/customer")]
        public async Task<ActionResult> GetWithdrawalByCustomerId([FromQuery] int customerId, [FromQuery] int? status)
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
                    var withdrawals = await _paymentRequestService.GetAllWithdrawalByCustomerId(customerId, status);
                    return Ok(withdrawals);
                }
                else
                {
                    return Forbid();
                } 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminOrTourGuideRole")]
        [HttpGet("withdrawal/tourguide")]
        public async Task<ActionResult> GetWithdrawalByTourGuideId([FromQuery] int tourGuideId, [FromQuery] int? status)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByTourGuideId(tourGuideId, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
                {
                    var withdrawals = await _paymentRequestService.GetAllWithdrawalByTourGuideId(tourGuideId, status);
                    return Ok(withdrawals);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("withdrawal")]
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
