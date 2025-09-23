using InventorySvc.Infrastructure.Inbox;
using InventorySvc.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
namespace InventorySvc.Infrastructure.Persistence;
public class InventoryDbContext(DbContextOptions<InventoryDbContext> o) : DbContext(o)
{
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    public DbSet<InventoryReservation> Reservations => Set<InventoryReservation>();
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
    public DbSet<InboxMessage>  Inbox  => Set<InboxMessage>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<InventoryItem>().HasKey(x=>x.ProductId);
        b.Entity<InventoryReservation>().HasKey(x=>x.Id);
        b.Entity<OutboxMessage>().HasKey(x=>x.Id);
        b.Entity<InboxMessage>().HasKey(x=>x.MessageId);
        base.OnModelCreating(b);
    }
}
