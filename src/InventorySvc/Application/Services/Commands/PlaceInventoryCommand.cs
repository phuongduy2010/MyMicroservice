namespace InventorySvc.Application.Services.Commands;
public record PlaceInventoryCommand(string ProductId, int TotalQuantity, int ReservedQuantity);