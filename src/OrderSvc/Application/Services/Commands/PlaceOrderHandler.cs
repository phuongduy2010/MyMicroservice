using Shared.Events;
using OrderSvc.Domain;
using OrderSvc.Application.Abstractions;
using OrderSvc.Domain.Entities;
using Shared;
namespace OrderSvc.Application.Services.Commands;

public sealed class PlaceOrderHandler(IOrderRepository orders, IOutboxRepository outbox, IUnitOfWork unitOfWork)
{
    private readonly IOrderRepository _orders = orders;
    private readonly IOutboxRepository _outbox = outbox;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<string> HandleAsync(PlaceOrderCommand cmd, CancellationToken ct)
    {

        await _unitOfWork.BeginAsync(ct);
        try
        {
            var order = Order.Create(cmd.CustomerId, cmd.Items);
            await _orders.AddAsync(order, ct);
            var evt = new OrderCreated(order.Id, order.CustomerId,
                cmd.Items.Select(i => new Shared.Events.OrderItem(i.ProductId, i.Qty)).ToList());
            await _outbox.AddAsync(Topic.OrderCreated, order.Id, evt, ct);
            await _unitOfWork.CommitAsync(ct);
            return order.Id;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
