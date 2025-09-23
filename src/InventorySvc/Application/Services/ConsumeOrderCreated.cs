using InventorySvc.Domain;
using Shared.Events;
namespace InventorySvc.Application.Services;
public sealed class ConsumeOrderCreated(IInventoryGateway gw)
{
    private readonly IInventoryGateway _gw = gw;
    public async Task HandleAsync(string messageId, OrderCreated evt, CancellationToken ct)
    {
        var items = evt.Items.Select(i=> new Domain.Entities.OrderItem(i.ProductId, i.Qty));
        var ok = await _gw.TryReserveAsync(evt.OrderId, items, ct);
        if (ok)
        {
            await _gw.RecordSuccessAsync(messageId, evt.OrderId, items, ct);
        }
        else
        {
            await _gw.RecordFailureAsync(messageId, evt.OrderId, "Insufficient stock", ct);
        }
    }
}
