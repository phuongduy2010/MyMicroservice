namespace ShippingSvc.Infrastructure.Persistence;
public class ShippingLabel
{
    public Guid Id { get; set; }
    public string OrderId { get; set; } = default!;
    public string TrackingNumber { get; set; } = default!;
    public string ShippingAddress { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = default!;
}