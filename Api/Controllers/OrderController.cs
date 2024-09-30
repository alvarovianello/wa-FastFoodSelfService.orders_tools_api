using Application.DTOs;
using Application.Interfaces.UseCases;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ICreateOrder _createOrder;
        private readonly IGetOrderById _getOrderById;
        private readonly IGetAllOrders _getAllOrders;
        private readonly IUpdateOrder _updateOrder;
        private readonly IDeleteOrder _deleteOrder;
        private readonly IUpdateOrderStatus _updateOrderStatus;
        private readonly IGetOrderByOrderNumber _getOrderByOrderNumber;
        private readonly IGetAllOrdersApproved _getAllOrdersApproved;
        private readonly IGetOrderByStatus _getOrderByStatus;
        private readonly IGetOrderSummaryById _getOrderSummaryById;

        public OrderController(
            ICreateOrder createOrder,
            IGetOrderById getOrderById,
            IGetAllOrders getAllOrders,
            IUpdateOrder updateOrder,
            IDeleteOrder deleteOrder,
            IUpdateOrderStatus updateOrderStatus,
            IGetOrderByOrderNumber getOrderByOrderNumber,
            IGetAllOrdersApproved getAllOrdersApproved,
            IGetOrderByStatus getOrderByStatus,
            IGetOrderSummaryById getOrderSummaryById)
        {
            _createOrder = createOrder;
            _getOrderById = getOrderById;
            _getAllOrders = getAllOrders;
            _updateOrder = updateOrder;
            _deleteOrder = deleteOrder;
            _updateOrderStatus = updateOrderStatus;
            _getOrderByOrderNumber = getOrderByOrderNumber;
            _getAllOrdersApproved = getAllOrdersApproved;
            _getOrderByStatus = getOrderByStatus;
            _getOrderSummaryById = getOrderSummaryById;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var order = await _createOrder.ExecuteAsync(request);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _getOrderById.ExecuteAsync(id);
                if (order == null) return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("summary/{orderNumber}")]
        public async Task<IActionResult> GetOrderSummaryById(string orderNumber)
        {
            try
            {
                var order = await _getOrderSummaryById.ExecuteAsync(orderNumber);
                if (order == null) return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _getAllOrders.ExecuteAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderRequest request)
        {
            try
            {
                if (id != request.OrderId) return BadRequest("ID do pedido não corresponde.");

                var updatedOrder = await _updateOrder.ExecuteAsync(request);

                if (updatedOrder == null) return NotFound();
                return Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _deleteOrder.ExecuteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] EnumOrderStatus status)
        {
            try
            {
                await _updateOrderStatus.ExecuteAsync(id, status);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("order-number/{orderNumber}")]
        public async Task<IActionResult> GetOrderByOrderNumber(string orderNumber)
        {
            try
            {
                var order = await _getOrderByOrderNumber.ExecuteAsync(orderNumber);
                if (order == null) return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("payment/approved")]
        public async Task<IActionResult> GetAllOrdersApproved(int limit = 20)
        {
            try
            {
                var orders = await _getAllOrdersApproved.ExecuteAsync(limit);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetOrderByStatus(EnumOrderStatus status)
        {
            try
            {
                var orders = await _getOrderByStatus.ExecuteAsync(status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
