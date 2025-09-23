namespace ShippingSvc.Api.Controllers;

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shared;
using ShippingSvc.Api.DTO;
using ShippingSvc.Application.Services.Commands;
using ShippingSvc.Application.Services.Queries;
using ShippingSvc.Domain.Entities;

[ApiController]
[Route("api/[controller]")]
public class ShippingController(
    GetShippingHandler getHandler,
    PlaceShippingHandler placeHandler) : ControllerBase
{
    private readonly GetShippingHandler _getHandler = getHandler;
    private readonly PlaceShippingHandler _placeHandler = placeHandler;

    [HttpGet("getShippings")]
    [ProducesResponseType(typeof(PagedResult<ShippingItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShippings(
        int page = 1, int pageSize = 20, string? orderId = null,
        CancellationToken ct = default)
    {
        var result = await _getHandler.GetShippings(page, pageSize, orderId, ct);
        return Ok(result);
    }

    [HttpPost("createShippings")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PlaceShipping(
        CreateShippingDto dto,
        CancellationToken ct = default)
    {
        var shipping = new PlaceShippingCommand(dto.OrderId, dto.TrackingNumber, dto.ShippingAddress);
        await _placeHandler.Handle(shipping, ct);
        return CreatedAtAction(nameof(GetShippings), new { id =  dto.TrackingNumber }, shipping);
    }
}