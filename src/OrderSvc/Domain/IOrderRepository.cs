
using OrderSvc.Domain.Entities;
namespace OrderSvc.Domain;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetByIdAsync(string orderId, CancellationToken ct);
    Task<int> CountAsync(
        string? customerId, string? orderStatus, DateTimeOffset? from, DateTimeOffset? to, string? search,
        CancellationToken ct);
    Task<IReadOnlyList<Order>> PageAsync(
        int skip, int take,
        string? customerId, string? status, DateTimeOffset? from, DateTimeOffset? to, string? search,
        CancellationToken ct);
}