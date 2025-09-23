using ShippingSvc.Domain.Entities;

namespace ShippingSvc.Domain;
public interface IShippingRepository
{
    Task AddAsync(ShippingItem item, CancellationToken ct);
    Task<int> CountAsync(string? orderId, CancellationToken ct);
    Task<IReadOnlyList<ShippingItem>> PageAsync(int skip, int take, string? orderId, CancellationToken ct);
}   
