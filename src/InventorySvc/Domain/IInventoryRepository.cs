using InventorySvc.Domain.Entities;

namespace InventorySvc.Domain;

public interface IInventoryRepository
{
    Task AddAsync(InventoryItem item, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<int> CountAsync(string? productId, CancellationToken ct);
    Task<IReadOnlyList<InventoryItem>> PageAsync(int skip, int take, string? productId, CancellationToken ct);
}