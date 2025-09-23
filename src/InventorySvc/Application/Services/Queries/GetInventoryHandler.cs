using InventorySvc.Domain;
using InventorySvc.Domain.Entities;
using Shared;

namespace InventorySvc.Application.Services.Queries;
public class GetInventoryHandler(IInventoryRepository repo)
{
    private readonly IInventoryRepository _repo = repo;
   public async Task<PagedResult<InventoryItem>> GetOrders(
        int page = 1, int pageSize = 20, string? productId = null,
        CancellationToken ct = default)
    {
        var size = Math.Clamp(pageSize, 1, 50);
        var skip = (page - 1) * size;
        var total = await _repo.CountAsync(productId, ct);
        var items = await _repo.PageAsync(skip, size, productId, ct);

        return new PagedResult<InventoryItem>
        {
            Items = items,
            Page = page,
            PageSize = size,
            TotalCount = total
        };
    }
}