using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Events;
using ShippingSvc.Infrastructure.Inbox;
using ShippingSvc.Infrastructure.Outbox;
using ShippingSvc.Infrastructure.Persistence;

namespace ShippingSvc.Infrastructure.Messaging;
public class OrderConfirmedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<string, string> _con;
    ILogger<OrderConfirmedConsumer> _logger;
    public OrderConfirmedConsumer(ILogger<OrderConfirmedConsumer> logger, IServiceScopeFactory scopeFactory, IConfiguration cfg)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _con = Kafka.Consumer(cfg["Kafka:BootstrapServers"] ?? "localhost:9092", "shipping-svc");
        _con.Subscribe(Topic.OrderConfirmed);
    }
    protected override Task ExecuteAsync(CancellationToken ct) => Task.Run(async () =>
    {
        while (!ct.IsCancellationRequested)
        {
            _logger.LogInformation("Shipping service Polling messages...");
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ShippingDbContext>();
            // blocks until a message arrives or the token is canceled.
            var cr = _con.Consume(ct);
            var msgId = $"{cr.Topic}:{cr.Partition.Value}:{cr.Offset.Value}";
            _logger.LogInformation("Shipping received {msgId}", msgId);
            if (await db.Inbox.FindAsync([msgId], ct) is not null)
            {
                _con.Commit(cr); continue;
            }
            var ev = JsonSerializer.Deserialize<OrderConfirmed>(cr.Message.Value)!;
            var shippingLabel = new ShippingLabel
            {
                OrderId = ev.OrderId,
                TrackingNumber = $"TRK-{Guid.NewGuid():N}"[0..12],
                ShippingAddress = "Emerald City",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            var payload = new ShipmentCreated(ev.OrderId, shippingLabel.TrackingNumber, ev.TraceId, 1);
            db.ShippingLabels.Add(shippingLabel);
            db.Outbox.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = Topic.ShipmentCreated,
                AggregateId = ev.OrderId,
                Payload = JsonSerializer.Serialize(payload),
                CreatedAt = DateTime.UtcNow
            });
            
            _logger.LogInformation("Shipping created {trackingNumber} for Order {OrderId}", shippingLabel.TrackingNumber, ev.OrderId);
            db.Inbox.Add(new InboxMessage { MessageId = msgId });
            await db.SaveChangesAsync(ct);
            _con.Commit(cr);
        }
    }, ct);
}
