using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentSvc.Infrastructure.Messaging;
using PaymentSvc.Infrastructure.Outbox;
using Shared;

namespace PaymentSvc.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        string connectionString,
        string bootstrapServers)
    {
        services.AddDbContext<PaymentDbContext>(o => o.UseNpgsql(connectionString));
        services.AddSingleton(Kafka.Producer(bootstrapServers));
        services.AddHostedService<OutboxDispatcher>();
        services.AddHostedService<InventoryReservedConsumer>();
        return services;
    }
}