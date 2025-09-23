using Microsoft.EntityFrameworkCore;
using PaymentSvc.Infrastructure.Inbox;
using PaymentSvc.Infrastructure.Outbox;
namespace PaymentSvc.Infrastructure;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> o) : DbContext(o)
{
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
    public DbSet<InboxMessage> Inbox => Set<InboxMessage>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<OutboxMessage>().HasKey(x => x.Id);
        b.Entity<InboxMessage>().HasKey(x => x.MessageId);
        base.OnModelCreating(b);
    }
}


