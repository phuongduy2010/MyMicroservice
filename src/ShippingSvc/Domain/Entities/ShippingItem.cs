using ShippingSvc.Domain.Enums;

namespace ShippingSvc.Domain.Entities;

public class ShippingItem
{
    public string OrderId { get; set; } = default!;
    public string TrackingNumber { get; set; } = default!;
    public string ShippingAddress { get; set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public ShippingStatus Status { get; set; } = ShippingStatus.LabelCreated;
    private ShippingItem() { }
    public  static ShippingItem Create(string orderId, string trackingNumber, string shippingAddress)
    {
         var item = new ShippingItem
        {
            OrderId = orderId,
            TrackingNumber = trackingNumber,
            ShippingAddress = shippingAddress,
        
        };
        return item;
    }
    public static ShippingItem Reconstruct(string OrderId, string TrackingNumber,
    string ShippingAddress, DateTime CreatedAt, ShippingStatus Status)
    {
        var order = new ShippingItem
        {
            OrderId = OrderId,
            TrackingNumber = TrackingNumber,
            ShippingAddress = ShippingAddress,
            CreatedAt = CreatedAt,
            Status = Status
        };
        return order;
    }
}