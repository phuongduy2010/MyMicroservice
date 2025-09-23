using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shared;
using Shared.Events;
namespace NotificationSvc.Infrastructure.Messaging;

public class NotificationConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _con;
    private readonly Dictionary<string, Func<string, string, Task>> _messageHandlers;
    public NotificationConsumer(IConfiguration cfg)
    {
        _con = Kafka.Consumer(cfg["Kafka:BootstrapServers"] ?? "localhost:9092", "notification-svc");
        _con.Subscribe([Topic.ShipmentCreated, Topic.OrderConfirmed, Topic.OrderCancelled]);
        _messageHandlers = new()
        {
            [Topic.ShipmentCreated] = HandleShipmentCreated,
            [Topic.OrderCancelled] = HandleOrderCancelled,
            [Topic.OrderConfirmed] = HandleOrderConfirmed
        };
    }
    protected override Task ExecuteAsync(CancellationToken ct) => Task.Run(async () =>
    {
        while (!ct.IsCancellationRequested)
        {
            var cr = _con.Consume(ct);
            var msgId = $"{cr.Topic}:{cr.Partition.Value}:{cr.Offset.Value}";
            var ev = JsonSerializer.Deserialize<ShipmentCreated>(cr.Message.Value)!;
            if (_messageHandlers.TryGetValue(cr.Topic, out var handler))
            {
                await handler(cr.Message.Value, msgId);
                _con.Commit(cr);
            }
        }
    }, ct);
    private Task HandleShipmentCreated(string messageValue, string msgId)
    {
        var ev = JsonSerializer.Deserialize<ShipmentCreated>(messageValue)!;
        Console.WriteLine($"[NotificationSvc] Shipment created for Order {ev.OrderId}, Tracking Number: {ev.TrackingNumber}");
        return Task.CompletedTask;
    }

    private Task HandleOrderConfirmed(string messageValue, string msgId)
    {
        var ev = JsonSerializer.Deserialize<OrderConfirmed>(messageValue)!;
        Console.WriteLine($"[NotificationSvc] Order {ev.OrderId} confirmed");
        return Task.CompletedTask;
    }
    private Task HandleOrderCancelled(string messageValue, string msgId)
    {
        var ev = JsonSerializer.Deserialize<OrderCancelled>(messageValue)!;
        Console.WriteLine($"[NotificationSvc] Order {ev.OrderId} cancelled, Reason: {ev.Reason}");
        return Task.CompletedTask;
    }
}