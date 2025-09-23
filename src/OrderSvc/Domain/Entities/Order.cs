using OrderSvc.Domain.Enums;
namespace OrderSvc.Domain.Entities;

public class Order
{
    public string Id { get; private set; } = default!;
    public string CustomerId { get; private set; } = default!;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items;
    private Order() { }
    public static Order Create(string customerId, IEnumerable<OrderItem> items)
    {
        var o = new Order { Id = Guid.NewGuid().ToString(), CustomerId = customerId };
        o._items.AddRange(items);
        return o;
    }
     public static Order Reconstruct(string id, string customerId, List<OrderItem> items, OrderStatus status)
    {
        var order = new Order { 
            Id = id, 
            CustomerId = customerId,
            Status = status 
        };
        order._items.AddRange(items);
        return order;
    }
}
public record OrderItem(string ProductId, int Qty);

