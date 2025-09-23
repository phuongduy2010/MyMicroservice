using InventorySvc.Domain;

namespace InventorySvc.Application.Services.Commands;
public class PlaceInventoryHandler(IInventoryRepository repo)
{
    private readonly IInventoryRepository _repo = repo;
    public async Task HandleAsync(PlaceInventoryCommand cmd, CancellationToken ct)
    {
        var item = new Domain.Entities.InventoryItem(cmd.ProductId, cmd.TotalQuantity, cmd.ReservedQuantity);
        await _repo.AddAsync(item, ct);
        await _repo.SaveChangesAsync(ct);
    }
}