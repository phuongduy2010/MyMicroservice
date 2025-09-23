using InventorySvc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventorySvc.Infrastructure;

public sealed class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        // Use env var first; fall back to a sensible default for local dev
        var conn = Environment.GetEnvironmentVariable("INVENTORY_DB")
                  ?? "Host=localhost;Port=5432;Database=inventory_db;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseNpgsql(conn)
            .Options;

        return new InventoryDbContext(options);
    }
}
