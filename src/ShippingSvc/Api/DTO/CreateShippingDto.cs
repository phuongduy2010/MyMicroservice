namespace ShippingSvc.Api.DTO;
public record CreateShippingDto(string OrderId, string TrackingNumber, string ShippingAddress);