using System.Text.Json;
using OrderSvc.Application.Abstractions;
using OrderSvc.Infrastructure.Persistence;

namespace OrderSvc.Infrastructure.Outbox;
public sealed class EfOutboxRepository(OrderDbContext db) : IOutboxRepository
{
    private readonly OrderDbContext _db = db;

    public async  Task AddAsync(string type, string aggregateId, object payload, CancellationToken ct)
    {
        var entity = new OutboxMessage
        {
            AggregateId = aggregateId,
            Type = type,
            Payload = JsonSerializer.Serialize(payload)
        };
        await _db.Outbox.AddAsync(entity, ct);
    }
}