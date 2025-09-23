using Shared;
using ShippingSvc.Domain;
using ShippingSvc.Domain.Entities;

namespace ShippingSvc.Application.Services.Queries;
public class GetShippingHandler(IShippingRepository repository)
{
    private readonly IShippingRepository _repository = repository;
    public async Task<PagedResult<ShippingItem>> GetShippings(
        int page = 1, int pageSize = 20, string? orderId = null,
        CancellationToken ct = default)
    {
        var size = Math.Clamp(pageSize, 1, 50);
        var skip = (page - 1) * size;
        var total = await _repository.CountAsync(orderId, ct);
        var items = await _repository.PageAsync(skip, size, orderId, ct);

        return new PagedResult<ShippingItem>
        {
            Items = items,
            Page = page,
            PageSize = size,
            TotalCount = total
        };
    }
}