
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PaymentSvc.Infrastructure.Outbox;

public class OutboxDispatcher(ILogger<OutboxDispatcher> logger, IServiceScopeFactory factory, IProducer<string, string> prod) : BackgroundService
{

    private readonly IServiceScopeFactory _factory = factory;
    private readonly IProducer<string, string> _prod = prod;
    private readonly ILogger<OutboxDispatcher> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            using var scope = _factory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
            var batch = await db.Outbox
            .Where(o => o.PublishedAt == null)
            .OrderBy(o => o.CreatedAt)
            .Take(200)
            .ToListAsync(ct);
            foreach (var m in batch)
            {
                await _prod.ProduceAsync(m.Type,
                new Message<string, string>
                {
                    Key = m.AggregateId,
                    Value = m.Payload
                }, ct);
                m.PublishedAt = DateTime.UtcNow;
                _logger.LogInformation("Published topic {Topic}", m.Type);
            }
            if (batch.Count > 0)
                await db.SaveChangesAsync(ct);
            await Task.Delay(250, ct);
        }
    }
}