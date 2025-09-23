using OrderSvc.Domain;
using OrderSvc.Domain.Entities;
using Shared;

namespace OrderSvc.Application.Services.Queries;
// Using primary constructor syntax
public sealed class GetOrderHandler(IOrderRepository orders)
{
    private readonly IOrderRepository _orders = orders;

    public async Task<Order?> GetOrderByIdAsync(string orderId, CancellationToken ct)
    {
        return await _orders.GetByIdAsync(orderId, ct);
    }

    public async Task<PagedResult<Order>> GetOrders(
        int page = 1, int pageSize = 20, string? status = null, string? customerId = null,
        DateTimeOffset? from = null, DateTimeOffset? to = null, string? search = null,
        CancellationToken ct = default)
    {
        var size = Math.Clamp(pageSize, 1, 50);
        var skip = (page - 1) * size;
        var total = await _orders.CountAsync(customerId, status, from, to, search, ct);
        var items = await _orders.PageAsync(skip, size, customerId, status, from, to, search, ct);

        return new PagedResult<Order>
        {
            Items = items,
            Page = page,
            PageSize = size,
            TotalCount = total
        };
    }
}