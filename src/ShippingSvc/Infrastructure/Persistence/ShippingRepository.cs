using Microsoft.EntityFrameworkCore;
using ShippingSvc.Domain;
using ShippingSvc.Domain.Entities;
using ShippingSvc.Domain.Enums;
using ShippingSvc.Infrastructure.Persistence;

class EfShippingRepository(ShippingDbContext db) : IShippingRepository
{
    private readonly ShippingDbContext _db = db;

    public Task AddAsync(ShippingItem shipment, CancellationToken cancellationToken = default)
    {
        _db.ShippingLabels.AddAsync(
            new ShippingLabel
            {
                OrderId = shipment.OrderId,
                TrackingNumber = shipment.TrackingNumber,
                ShippingAddress = shipment.ShippingAddress
            }, cancellationToken);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(string? orderId, CancellationToken ct)
    {
        return await _db.ShippingLabels
           .AsNoTracking()
           .Where(x => orderId == null || x.OrderId == orderId)
           .CountAsync(ct);
    }

    public async Task<IReadOnlyList<ShippingItem>> PageAsync(int skip, int take, string? orderId, CancellationToken ct)
    {
        return await _db.ShippingLabels
           .AsNoTracking()
           .Where(x => orderId == null || x.OrderId == orderId)
           .OrderBy(x => x.CreatedAt)
           .Skip(skip)
           .Take(take)
           .Select(x => ConvertEntityToOrder(x))
           .ToListAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _db.SaveChangesAsync(ct);
    }
    private static ShippingItem ConvertEntityToOrder(ShippingLabel shipping)
    {
        var status = Enum.Parse<ShippingStatus>(shipping.Status);
        return ShippingItem.Reconstruct(
            shipping.OrderId,
            shipping.TrackingNumber,
            shipping.ShippingAddress,
            shipping.CreatedAt,
            status
    );
    }
}