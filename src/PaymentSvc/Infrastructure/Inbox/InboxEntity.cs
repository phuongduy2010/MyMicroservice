namespace PaymentSvc.Infrastructure.Inbox;
public class InboxMessage
{
    public string MessageId { get; set; } = default!;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}