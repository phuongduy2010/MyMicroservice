using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using ShippingSvc.Application.Abstractions;
using ShippingSvc.Domain;
using ShippingSvc.Infrastructure.Messaging;
using ShippingSvc.Infrastructure.Outbox;
using ShippingSvc.Infrastructure.Persistence;

namespace ShippingSvc.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string bootstrapServers)
    {
        services.AddDbContext<ShippingDbContext>(o => o.UseNpgsql(connectionString));
        services.AddScoped<IShippingRepository, EfShippingRepository>();
        services.AddScoped<IOutboxRepository, EfOutboxRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddHostedService<OutboxDispatcher>();
        services.AddSingleton(Kafka.Producer(bootstrapServers));
        services.AddHostedService<OrderConfirmedConsumer>();
        return services;
    }
}