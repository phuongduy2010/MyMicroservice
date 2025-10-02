using OrderSvc.Application.Abstractions;
using OrderSvc.Infrastructure.Persistence;

namespace OrderSvc.Infrastructure.Inbox;

public sealed class EfInboxRepository(OrderDbContext orderDbContext) : IInboxRepository
{

    private readonly OrderDbContext _orderDbContext = orderDbContext;
    public async Task<bool> ExistsAsync(string messageId, CancellationToken ct = default)
    {
        bool exist = await _orderDbContext.Inbox.FindAsync([messageId], ct) != null;
        return exist;
    }

    public async Task MarkProcessedAsync(string messageId, CancellationToken ct = default)
    {
        InboxMessage message = new() { MessageId = messageId };
        await _orderDbContext.Inbox.AddAsync(message, ct);
    }
}