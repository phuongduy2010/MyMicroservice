
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderSvc.Domain;
using OrderSvc.Domain.Entities;
using OrderSvc.Domain.Enums;
namespace OrderSvc.Infrastructure.Persistence;

public sealed class EfOrderRepository(OrderDbContext db) : IOrderRepository
{
    private readonly OrderDbContext _db = db;
    public async Task AddAsync(Order order, CancellationToken ct)
    {
        var entity = new OrderEntity
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = OrderStatus.Pending.ToString(),
            ItemsJson = JsonSerializer.Serialize(order.Items)
        };
        await _db.Orders.AddAsync(entity, ct);
    }
    public async Task<Order?> GetByIdAsync(string id, CancellationToken ct)
    {
        var e = await _db.Orders.FindAsync([id], ct);
        if (e is null) return null;
        return ConvertEntityToOrder(e);
    }
    public async Task<int> CountAsync(
        string? customerId, string? status, DateTimeOffset? from, DateTimeOffset? to, string? search,
        CancellationToken ct)
    {
        var q = _db.Orders.AsNoTracking();
        q = ApplyFilters(q, customerId, status, from, to, search);
        return await q.CountAsync(ct);
    }
    public async Task<IReadOnlyList<Order>> PageAsync(
      int skip, int take,
      string? customerId, string? status, DateTimeOffset? from, DateTimeOffset? to, string? search,
      CancellationToken ct)
    {
        var q = _db.Orders.AsNoTracking();
        q = ApplyFilters(q, customerId, status, from, to, search);
        return await q
            .OrderByDescending(o => o.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(e => ConvertEntityToOrder(e)).ToListAsync(ct);
    }
    private static IQueryable<OrderEntity> ApplyFilters(
            IQueryable<OrderEntity> q,
            string? customerId, string? status,
            DateTimeOffset? from,
            DateTimeOffset? to, string? search)
    {
        if (!string.IsNullOrWhiteSpace(customerId)) q = q.Where(o => o.CustomerId == customerId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            q = q.Where(o => o.Status == status);
        }

        if (from.HasValue)
        {
            q = q.Where(o => o.CreatedAt >= from.Value);
        }
        if (to.HasValue)
        {
            q = q.Where(o => o.CreatedAt <= to.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            q = q.Where(o => o.Id.Contains(search) || o.CustomerId.Contains(search));
        }
        return q;
    }
    private static Order ConvertEntityToOrder(OrderEntity e)
    {
        var items = JsonSerializer.Deserialize<List<OrderItem>>(e.ItemsJson)!;
        var status = Enum.Parse<OrderStatus>(e.Status);
        return Order.Reconstruct(e.Id, e.CustomerId, items, status);
    }   
}

