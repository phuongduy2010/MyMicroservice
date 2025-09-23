using InventorySvc.Domain.Entities;

namespace InventorySvc.Domain;
public interface IInventoryGateway
{
    Task<bool> TryReserveAsync(string orderId, IEnumerable<OrderItem> items, CancellationToken ct);
    Task RecordSuccessAsync(string messageId, string orderId, IEnumerable<OrderItem> items, CancellationToken ct);
    Task RecordFailureAsync(string messageId, string orderId, string reason, CancellationToken ct);
}
