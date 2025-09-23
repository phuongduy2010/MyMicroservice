using InventorySvc.Domain;
using InventorySvc.Infrastructure.Messaging;
using InventorySvc.Infrastructure.Outbox;
using InventorySvc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace InventorySvc.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        string connectionString,
        string bootstrapServers)
    {
        services.AddDbContext<InventoryDbContext>(o => o.UseNpgsql(connectionString));
        services.AddScoped<IInventoryGateway, EfInventoryGateway>();
        services.AddScoped<IInventoryRepository, EfInventoryRepository>();
        services.AddSingleton(Kafka.Producer(bootstrapServers));
        services.AddHostedService<OutboxDispatcher>();
        services.AddHostedService<OrderCreatedConsumer>();
        return services;
    }
}