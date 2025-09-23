using Shared;
using ShippingSvc.Application.Abstractions;
using ShippingSvc.Domain;
using ShippingSvc.Domain.Entities;
namespace ShippingSvc.Application.Services.Commands;

public class PlaceShippingHandler(IShippingRepository repository,IOutboxRepository outbox, IUnitOfWork unitOfWork)
{
    private readonly IShippingRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IOutboxRepository _outbox = outbox;
    public async Task<string> Handle(PlaceShippingCommand command, CancellationToken cancellationToken)
    {
        var shippingItem = ShippingItem.Create(command.OrderId, command.TrackingNumber, command.ShippingAddress);
        await _repository.AddAsync(shippingItem, cancellationToken);
        await _outbox.AddAsync(Topic.ShipmentCreated, shippingItem.OrderId, shippingItem, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return shippingItem.TrackingNumber;
    }
}