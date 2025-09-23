
using Confluent.Kafka;
using InventorySvc.Application.Services;
using InventorySvc.Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InventorySvc.Infrastructure.Messaging;
public sealed class OrderCreatedConsumer : BackgroundService
{   
     private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<string, string> _con;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IServiceScopeFactory scopeFactory, IConfiguration cfg)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _con = Kafka.Consumer(cfg["Kafka:BootstrapServers"] ?? "localhost:9092", "inventory-svc");
        _con.Subscribe(Topic.OrderCreated);
    }
    protected override Task ExecuteAsync(CancellationToken ct) => Task.Run(async () =>
    {
        while (!ct.IsCancellationRequested)
        {
            _logger.LogInformation("Inventory is Polling for messages...");
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            var useCase = scope.ServiceProvider.GetRequiredService<ConsumeOrderCreated>();
            // blocks until a message arrives or the token is canceled.
            var cr = _con.Consume(ct);
            var msgId = $"{cr.Topic}:{cr.Partition.Value}:{cr.Offset.Value}";
            _logger.LogInformation("Inventory received {msgId}", msgId);
            if (await db.Inbox.FindAsync([msgId], ct) is not null)
            {
                _con.Commit(cr); continue;
            }
            var evt = System.Text.Json.JsonSerializer.Deserialize<OrderCreated>(cr.Message.Value)!;
            await useCase.HandleAsync(msgId, evt, ct);
            _con.Commit(cr);
        }
    }, ct);
}