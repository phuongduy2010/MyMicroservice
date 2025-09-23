using System.Text.Json;
using ShippingSvc.Application.Abstractions;
using ShippingSvc.Infrastructure.Persistence;

namespace ShippingSvc.Infrastructure.Outbox;
public class EfOutboxRepository(ShippingDbContext db) : IOutboxRepository
{
    private readonly ShippingDbContext _db = db;

    public async Task AddAsync(string type, string aggregateId, object payload, CancellationToken ct)
    {
        await _db.Outbox.AddAsync(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = type,
            AggregateId = aggregateId,
            Payload = JsonSerializer.Serialize(payload),
            CreatedAt = DateTime.UtcNow
        }, ct);
    }
}