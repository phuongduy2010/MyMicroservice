using InventorySvc.Application.Services.Commands;
using InventorySvc.Application.Services.Queries;
using InventorySvc.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared;
namespace InventorySvc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController(ILogger<InventoryController> logger,
GetInventoryHandler getHandler, PlaceInventoryHandler handler) : ControllerBase
{
    private readonly ILogger<InventoryController> _logger = logger;
    private readonly GetInventoryHandler _getHandler = getHandler;
    private readonly PlaceInventoryHandler _handler = handler;
    [HttpPost]
    public async Task<IActionResult> PlaceInventory(PlaceInventoryCommand cmd, CancellationToken ct)
    {
        await _handler.HandleAsync(cmd, ct);
        _logger.LogInformation("Inventory placed: {InventoryItem}", cmd);
        return CreatedAtAction(nameof(Get), new { productId = cmd.ProductId }, cmd);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<InventoryItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<InventoryItem>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? productId = null,
        CancellationToken ct = default)
    {
        var result = await _getHandler.GetOrders(page, pageSize, productId, ct);
        return Ok(result);
    }
}