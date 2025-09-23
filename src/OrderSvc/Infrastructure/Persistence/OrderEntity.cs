namespace OrderSvc.Infrastructure.Persistence;

public class OrderEntity
{
    public string Id { get; set; } = default!;
    public string CustomerId { get; set; } = default!;
    public string Status { get; set; } = "PENDING";
    public string ItemsJson { get; set; } = "[]";
     public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
