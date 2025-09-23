using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace PaymentSvc.Infrastructure;

public sealed class PaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
{
    public PaymentDbContext CreateDbContext(string[] args)
    {
        // Use env var first; fall back to a sensible default for local dev
        var conn = Environment.GetEnvironmentVariable("PAYMENT_DB")
                  ?? "Host=localhost;Port=5432;Database=payment_db;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseNpgsql(conn)
            .Options;

        return new PaymentDbContext(options);
    }
}
