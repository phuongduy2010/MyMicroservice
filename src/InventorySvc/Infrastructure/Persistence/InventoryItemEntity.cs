namespace InventorySvc.Infrastructure.Persistence;
public class InventoryItem
{
    public string ProductId { get; set; } = default!;
    public int TotalQuantity { get; set; }
    public int ReservedQuantity { get; set; }
}