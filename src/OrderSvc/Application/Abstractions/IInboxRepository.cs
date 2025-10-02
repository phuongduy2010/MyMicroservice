namespace OrderSvc.Application.Abstractions;

public interface IInboxRepository
{
    Task<bool> ExistsAsync(string messageId, CancellationToken ct = default);
    Task MarkProcessedAsync(string messageId, CancellationToken ct = default);
}
