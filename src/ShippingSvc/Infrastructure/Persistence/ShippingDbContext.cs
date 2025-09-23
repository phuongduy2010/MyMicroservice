using Microsoft.EntityFrameworkCore;
using ShippingSvc.Infrastructure.Inbox;
using ShippingSvc.Infrastructure.Outbox;

namespace ShippingSvc.Infrastructure.Persistence;

public class ShippingDbContext(DbContextOptions<ShippingDbContext> o) : DbContext(o)
{
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
    public DbSet<InboxMessage> Inbox => Set<InboxMessage>();
     public DbSet<ShippingLabel> ShippingLabels => Set<ShippingLabel>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<OutboxMessage>().HasKey(x => x.Id);
        b.Entity<InboxMessage>().HasKey(x => x.MessageId);
        b.Entity<ShippingLabel>().HasKey(x => x.Id);
        base.OnModelCreating(b);
    }
}
