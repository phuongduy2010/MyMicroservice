using System.Net;
using OrderSvc.Application.Services.Queries;
using Microsoft.AspNetCore.Mvc;
using OrderSvc.Api.DTO;
using OrderSvc.Application.Services.Commands;
using OrderSvc.Domain.Entities;

using Shared;
namespace OrderSvc.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(ILogger<OrderController> logger, PlaceOrderHandler handler, GetOrderHandler getHandler) : ControllerBase
    {
        private readonly PlaceOrderHandler _handler = handler;
        private readonly GetOrderHandler _getHandler = getHandler;
        private readonly ILogger<OrderController> _logger = logger;

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto, CancellationToken ct)
        {
            var order = new PlaceOrderCommand(dto.CustomerId, dto.Items.Select(i => new OrderItem(i.ProductId, i.Qty)).ToList());
            var orderId = await _handler.HandleAsync(order, ct);
            _logger.LogInformation("Order created: {OrderId}", orderId);
            return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, order);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(Guid id)
        {
            var order = await _getHandler.GetOrderByIdAsync(id.ToString(), CancellationToken.None);
            _logger.LogInformation("Get OrderId: {OrderId}", id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<Order>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<Order>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? customerId = null,
            [FromQuery] DateTimeOffset? from = null,
            [FromQuery] DateTimeOffset? to = null,
            [FromQuery] string? search = null,
            CancellationToken ct = default)
        {
            var result = await _getHandler.GetOrders(page, pageSize, status,
            customerId, from, to, search, ct);
            return Ok(result);
        }

    }
}