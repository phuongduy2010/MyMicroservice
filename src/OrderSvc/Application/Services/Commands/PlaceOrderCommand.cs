using OrderSvc.Domain.Entities;
namespace OrderSvc.Application.Services.Commands;
public sealed record PlaceOrderCommand(string CustomerId, List<OrderItem> Items);