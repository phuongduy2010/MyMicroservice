using InventorySvc.Domain;
using Microsoft.EntityFrameworkCore;
namespace InventorySvc.Infrastructure.Persistence;
public class EfInventoryRepository(InventoryDbContext db) : IInventoryRepository
{
    private readonly InventoryDbContext _db = db;

    public Task AddAsync(Domain.Entities.InventoryItem item, CancellationToken ct)
    {
        _db.Inventory.Add(new InventoryItem
        {
            ProductId = item.ProductId,
            TotalQuantity = item.TotalQuantity,
            ReservedQuantity = item.ReservedQuantity
        });
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(string? productId, CancellationToken ct)
    {
        return await _db.Inventory
            .AsNoTracking()
            .Where(x => productId == null || x.ProductId == productId)
            .CountAsync(ct);
    }

    public async Task<IReadOnlyList<Domain.Entities.InventoryItem>> PageAsync(int skip, int take, string? productId, CancellationToken ct)
    {
        return await _db.Inventory
            .AsNoTracking()
            .Where(x => productId == null || x.ProductId == productId)
            .OrderBy(x => x.ProductId)
            .Skip(skip)
            .Take(take)
            .Select(x => new Domain.Entities.InventoryItem(
                x.ProductId,
                x.TotalQuantity,
                x.ReservedQuantity))
            .ToListAsync(ct);

    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _db.SaveChangesAsync(ct);
    }
}