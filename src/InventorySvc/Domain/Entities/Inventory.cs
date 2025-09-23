namespace InventorySvc.Domain.Entities;
public class InventoryItem(string productId, int totalQuantity, int reservedQuantity)
{
    public string ProductId { get; set; } = productId;
    public int TotalQuantity { get; set; } = totalQuantity;
    public int ReservedQuantity { get; set; } = reservedQuantity;
}