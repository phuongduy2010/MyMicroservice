using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ShippingSvc.Infrastructure.Persistence;
namespace ShippingSvc.Infrastructure;

public sealed class ShippingDbContextFactory : IDesignTimeDbContextFactory<ShippingDbContext>
{
    public ShippingDbContext CreateDbContext(string[] args)
    {
        // Use env var first; fall back to a sensible default for local dev
        var conn = Environment.GetEnvironmentVariable("SHIPPING_DB")
                  ?? "Host=localhost;Port=5432;Database=shipping_db;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<ShippingDbContext>()
            .UseNpgsql(conn)
            .Options;

        return new ShippingDbContext(options);
    }
}
