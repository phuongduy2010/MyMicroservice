using Microsoft.EntityFrameworkCore;
using OrderSvc.Infrastructure.Outbox;
namespace OrderSvc.Infrastructure.Persistence;
public class OrderDbContext(DbContextOptions<OrderDbContext> o) : DbContext(o)
{
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
    public DbSet<InboxMessage>  Inbox  => Set<InboxMessage>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<OrderEntity>().HasKey(x=>x.Id);
        b.Entity<OutboxMessage>().HasKey(x=>x.Id);
        b.Entity<InboxMessage>().HasKey(x=>x.MessageId);
        base.OnModelCreating(b);
    }
}