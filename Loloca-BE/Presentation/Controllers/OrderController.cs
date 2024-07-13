using Loloca_BE.Business.Models.OrderView;
using Loloca_BE.Business.Services.Implements;
using Loloca_BE.Business.Services.Interfaces;
using Loloca_BE.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAuthorizeService _authorizeService;

        public OrderController(IOrderService orderService, IAuthorizeService authorizeService)
        {
            _orderService = orderService;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("get-all-order")]
        public async Task<ActionResult<IEnumerable<OrderModelView>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminOrCustomerRole")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderModelView>> GetOrderByIdAsync(int id)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByOrderId(id, int.Parse(accountId));
                if (checkAuthorize.isUser || checkAuthorize.isAdmin)
                {
                    var order = await _orderService.GetOrderByIdAsync(id);
                    if (order == null)
                    {
                        return NotFound();
                    }
                    return Ok(order);
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

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("tourGuide")]
        public async Task<ActionResult<OrderForBookingTourGuideView>> CreateOrderForBookingTourGuideRequestAsync(OrderForBookingTourGuideView orderModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByBookingTourGuideRequestId((int) orderModel.BookingTourGuideRequestId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    var createdOrder = await _orderService.CreateOrderForBookingTourGuideRequestAsync(orderModel);
                    if (createdOrder == null)
                    {
                        return BadRequest("Failed to create order.");
                    }

                    return Ok(createdOrder); // Return the created order with HTTP status 200 (OK)
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


        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("tour")]
        public async Task<ActionResult<OrderForBookingTourView>> CreateOrderForBookingTourRequestAsync(OrderForBookingTourView orderModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByBookingTourRequestId((int)orderModel.BookingTourRequestsId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    var createdOrder = await _orderService.CreateOrderForBookingTourRequestAsync(orderModel);
                    if (createdOrder == null)
                    {
                        return BadRequest("Failed to create order.");
                    }

                    return Ok(createdOrder); // Return the created order with HTTP status 200 (OK)
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

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("update-status")]
        public async Task<ActionResult<OrderModelView>> UpdateOrderStatusAsync([FromBody] UpdateOrderStatusView updateOrderStatusView)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkAuthorize = await _authorizeService.CheckAuthorizeByOrderId(updateOrderStatusView.OrderId, int.Parse(accountId));
                if (checkAuthorize.isUser)
                {
                    var updatedOrder = await _orderService.UpdateOrderStatusAsync(updateOrderStatusView);
                    return Ok(updatedOrder);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $" Internal Server Error: {ex.Message}");
            }
        }

    }
}
