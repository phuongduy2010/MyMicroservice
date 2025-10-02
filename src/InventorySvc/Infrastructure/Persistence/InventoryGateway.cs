using InventorySvc.Domain;
using InventorySvc.Infrastructure.Inbox;
using InventorySvc.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Events;

namespace InventorySvc.Infrastructure.Persistence;
public sealed class EfInventoryGateway(InventoryDbContext db) : IInventoryGateway
{
    private readonly InventoryDbContext _db = db;

    public async Task<bool> TryReserveAsync(string orderId, IEnumerable<Domain.Entities.OrderItem> items, CancellationToken ct)
    {
        using var tx = await _db.Database.BeginTransactionAsync(ct);
        foreach (var item in items)
        {
            var rows = await _db.Database.ExecuteSqlRawAsync(@"
                UPDATE ""Inventory""
                   SET ""ReservedQuantity"" = ""ReservedQuantity"" + {0}
                 WHERE ""ProductId"" = {1}
                   AND (""TotalQuantity"" - ""ReservedQuantity"") >= {0};",
                   item.Qty, item.ProductId);

            // var inventoryItem = await _db.Inventory
            //                     .Where(i => i.ProductId == item.ProductId && (i.TotalQuantity - i.ReservedQuantity) >= item.Qty)
            //                     .FirstOrDefaultAsync();
            // if (inventoryItem != null)
            // {
            //     inventoryItem.ReservedQuantity += item.Qty;
            //     await _db.SaveChangesAsync();
            // }
            if (rows == 0)
            {
                await tx.RollbackAsync(ct);
                return false;
            }
        }
        foreach (var item in items)
        {
            _db.Reservations.Add(new InventoryReservation
            {
                OrderId = orderId,
                ProductId = item.ProductId,
                Quantity = item.Qty
            });
        }
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return true;
    }
    public async Task RecordSuccessAsync(string messageId, string orderId, IEnumerable<Domain.Entities.OrderItem> items, CancellationToken ct)
    {
        _db.Inbox.Add(new InboxMessage { MessageId = messageId });
        var payload = new InventoryReserved(orderId, items.Select(x => new OrderItem(x.ProductId, x.Qty)).ToList());
        _db.Outbox.Add(new OutboxMessage
        {
            AggregateId = orderId,
            Type = Topic.InventoryReserved,
            Payload = System.Text.Json.JsonSerializer.Serialize(payload)
        });
        await _db.SaveChangesAsync(ct);
    }
    public async Task RecordFailureAsync(string messageId, string orderId, string reason, CancellationToken ct)
    {
        _db.Inbox.Add(new InboxMessage { MessageId = messageId });
        var payload = new InventoryFailed(orderId, reason);
        _db.Outbox.Add(new OutboxMessage
        {
            AggregateId = orderId,
            Type = Topic.InventoryFailed,
            Payload = System.Text.Json.JsonSerializer.Serialize(payload)
        });
        await _db.SaveChangesAsync(ct);
    }
}
