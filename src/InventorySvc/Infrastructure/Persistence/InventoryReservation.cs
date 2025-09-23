namespace InventorySvc.Infrastructure.Persistence;
public class InventoryReservation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OrderId { get; set; } = default!;
    public string ProductId { get; set; } = default!;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}