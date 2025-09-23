namespace ShippingSvc.Application.Abstractions;
public interface IOutboxRepository
{
    Task AddAsync(string type, string aggregateId, object payload, CancellationToken ct);
}