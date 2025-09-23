using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OrderSvc.Infrastructure.Persistence;

namespace OrderSvc.Infrastructure;

public sealed class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContext CreateDbContext(string[] args)
    {
        // Use env var first; fall back to a sensible default for local dev
        var conn = Environment.GetEnvironmentVariable("ORDER_DB")
                  ?? "Host=localhost;Port=5432;Database=order_db;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseNpgsql(conn)
            .Options;

        return new OrderDbContext(options);
    }
}
