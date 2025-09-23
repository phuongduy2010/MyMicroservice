namespace ShippingSvc.Application.Services.Commands;
public record PlaceShippingCommand(string OrderId, string TrackingNumber, string ShippingAddress);