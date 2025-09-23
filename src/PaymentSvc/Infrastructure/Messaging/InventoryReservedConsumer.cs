using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentSvc.Infrastructure.Inbox;
using PaymentSvc.Infrastructure.Outbox;
using Shared;
using Shared.Events;

namespace PaymentSvc.Infrastructure.Messaging;
public class InventoryReservedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _factory;
    private readonly IConsumer<string, string> _con;
    private readonly ILogger<InventoryReservedConsumer> _logger;
    public InventoryReservedConsumer(ILogger<InventoryReservedConsumer> logger, IServiceScopeFactory factory, IConfiguration cfg)
    {
        _factory = factory;
        _logger = logger;
        _con = Kafka.Consumer(cfg["Kafka:BootstrapServers"] ?? "localhost:9092", "payment-svc");
        _con.Subscribe(Topic.InventoryReserved);
    }
    protected override Task ExecuteAsync(CancellationToken ct) => Task.Run(async () =>
    {
        while (!ct.IsCancellationRequested)
        {
            _logger.LogInformation("Payment service is Polling messages...");
            using var scope = _factory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
            // blocks until a message arrives or the token is canceled.
            var cr = _con.Consume(ct);
            var msgId = $"{cr.Topic}:{cr.Partition.Value}:{cr.Offset.Value}";
            _logger.LogInformation("Payment received {msgId}", msgId);
            if (await db.Inbox.FindAsync([msgId], ct) is not null)
            {
                _con.Commit(cr); continue;
            }
            var ev = JsonSerializer.Deserialize<InventoryReserved>(cr.Message.Value)!;
            var pa = new PaymentAuthorized(ev.OrderId, $"PAY-{Guid.NewGuid():N}"[0..12], 100);
            db.Outbox.Add(new OutboxMessage
            {
                AggregateId = ev.OrderId,
                Type =Topic.PaymentAuthorized,
                Payload = JsonSerializer.Serialize(pa)
            });
            db.Inbox.Add(new InboxMessage { MessageId = msgId });
            await db.SaveChangesAsync(ct);
            _con.Commit(cr);
        }
    }, ct);
}
