namespace Shared;

public static class Topic
{
    public const string OrderCreated = "order-created";
    public const string OrderCancelled = "order-cancelled";
    public const string OrderConfirmed = "order-confirmed";
    public const string InventoryReserved = "inventory.reserved";
    public const string InventoryFailed = "inventory.failed";
    public const string PaymentAuthorized = "payment.authorized";
    public const string PaymentFailed = "payment.failed";
    
    public const string ShipmentCreated = "shipment.created";
}
