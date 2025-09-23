namespace OrderSvc.Api.DTO;
public record CreateOrderDto(string CustomerId, List<ItemDto> Items);
public record ItemDto(string ProductId, int Qty);