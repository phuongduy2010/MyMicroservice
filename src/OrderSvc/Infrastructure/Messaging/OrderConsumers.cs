using Microsoft.Extensions.Hosting;
using OrderSvc.Infrastructure.Persistence;
using Confluent.Kafka;

using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Shared;
using Shared.Events;
using OrderSvc.Infrastructure.Outbox;
using OrderSvc.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace OrderSvc.Infrastructure.Messaging;

public sealed class OrderConsumers : BackgroundService
{
    
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<string, string> _con;
    private readonly Dictionary<string, Func<string, string, Task>> _messageHandlers;

    private readonly ILogger<OrderConsumers> _logger;
    public OrderConsumers(IServiceScopeFactory scopeFactory, ILogger<OrderConsumers> logger, IConfiguration cfg)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _con = Kafka.Consumer(cfg["Kafka:BootstrapServers"] ?? "localhost:9092", "orders-svc");
        _con.Subscribe([Topic.InventoryReserved, Topic.InventoryFailed, Topic.PaymentAuthorized, Topic.PaymentFailed]);
        _messageHandlers = new()
        {
            [Topic.InventoryReserved] = HandleInventoryReserved,
            [Topic.InventoryFailed] = HandleInventoryFailed,
            [Topic.PaymentAuthorized] = HandlePaymentAuthorized,
            [Topic.PaymentFailed] = HandlePaymentFailed
        };
    }
    // BackgroundService need a long running task
    // Keep the Kafka consumer polling in a separate thread
    // Prevent blocking the main thread
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        return Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                _logger.LogInformation("Order service is Polling messages...");
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                 // blocks until a message arrives or the token is canceled.
                var cr = _con.Consume(ct);
                var msgId = $"{cr.Topic}:{cr.Partition.Value}:{cr.Offset.Value}";
               _logger.LogInformation("Order received {msgId}", msgId);
                if (await db.Inbox.FindAsync([msgId]) != null)
                {
                    _con.Commit(cr);
                    continue;
                }
                if (_messageHandlers.TryGetValue(cr.Topic, out var handler))
                {
                    await handler(cr.Message.Value, msgId);
                    _con.Commit(cr);
                }
            }
        }, ct);
    }
    private async Task HandleInventoryReserved(string messageValue, string msgId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var ev = JsonSerializer.Deserialize<InventoryReserved>(messageValue)!;
        var order = await db.Orders.FindAsync([ev.OrderId]);
        if (order != null) order.Status =  OrderStatus.AwaitingPayment.ToString();
        db.Inbox.Add(new InboxMessage { MessageId = msgId });
        await db.SaveChangesAsync();
    }

    private async Task HandleInventoryFailed(string messageValue, string msgId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var ev = JsonSerializer.Deserialize<InventoryFailed>(messageValue)!;
        var order = await db.Orders.FindAsync([ev.OrderId]);
        if (order != null) order.Status = OrderStatus.Cancelled.ToString();
        db.Outbox.Add(new OutboxMessage
        {
            AggregateId = ev.OrderId,
            Type = Topic.OrderCancelled,
            Payload = JsonSerializer.Serialize(new OrderCancelled(ev.OrderId, ev.Reason))
        });
        db.Inbox.Add(new InboxMessage { MessageId = msgId });
        await db.SaveChangesAsync();
    }

    private async Task HandlePaymentAuthorized(string messageValue, string msgId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var ev = JsonSerializer.Deserialize<PaymentAuthorized>(messageValue)!;
        var order = await db.Orders.FindAsync([ev.OrderId]);
        if (order != null) order.Status =  OrderStatus.Confirmed.ToString();
        db.Outbox.Add(new OutboxMessage
        {
            AggregateId = ev.OrderId,
            Type = Topic.OrderConfirmed,
            Payload = JsonSerializer.Serialize(new OrderConfirmed(ev.OrderId))
        });
        db.Inbox.Add(new InboxMessage { MessageId = msgId });
        await db.SaveChangesAsync();
    }

    private async Task HandlePaymentFailed(string messageValue, string msgId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var ev = JsonSerializer.Deserialize<PaymentFailed>(messageValue)!;
        var order = await db.Orders.FindAsync([ev.OrderId]);
        if (order != null) order.Status =  OrderStatus.Cancelled.ToString();
        db.Outbox.Add(new OutboxMessage
        {
            AggregateId = ev.OrderId,
            Type = Topic.OrderCancelled,
            Payload = JsonSerializer.Serialize(new OrderCancelled(ev.OrderId, ev.Reason))
        });
        db.Inbox.Add(new InboxMessage { MessageId = msgId });
        await db.SaveChangesAsync();
    }
}