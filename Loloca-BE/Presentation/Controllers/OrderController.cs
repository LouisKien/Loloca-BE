using Loloca_BE.Business.Models.OrderView;
using Loloca_BE.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Loloca_BE.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderModelView>>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderModelView>> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("tourGuide")]
        public async Task<ActionResult<OrderModelView>> CreateOrderForBookingTourGuideRequestAsync(OrderModelView orderModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdOrder = await _orderService.CreateOrderForBookingTourGuideRequestAsync(orderModel);
                if (createdOrder == null)
                {
                    return BadRequest("Failed to create order.");
                }

                return Ok(createdOrder); // Return the created order with HTTP status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpPost("tour")]
        public async Task<ActionResult<OrderModelView>> CreateOrderForBookingTourRequestAsync(OrderModelView orderModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdOrder = await _orderService.CreateOrderForBookingTourRequestAsync(orderModel);
                if (createdOrder == null)
                {
                    return BadRequest("Failed to create order.");
                }

                return Ok(createdOrder); // Return the created order with HTTP status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/status/{status}")]
        public async Task<ActionResult<OrderModelView>> UpdateOrderStatusAsync(int id, int status)
        {
            try
            {
                var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, status);
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
