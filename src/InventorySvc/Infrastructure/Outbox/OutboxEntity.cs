
namespace InventorySvc.Infrastructure.Outbox;
public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AggregateId { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
}
