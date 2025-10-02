using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderSvc.Application.Abstractions;
using OrderSvc.Infrastructure.Persistence;
using OrderSvc.Infrastructure.Messaging;
using OrderSvc.Infrastructure.Outbox;
using OrderSvc.Domain;
using Shared;
using OrderSvc.Infrastructure.Inbox;

namespace OrderSvc.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        string connectionString,
        string bootstrapServers)
    {
        services.AddDbContext<OrderDbContext>(o => o.UseNpgsql(connectionString));
        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<IOutboxRepository, EfOutboxRepository>();
        services.AddScoped<IInboxRepository, EfInboxRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddSingleton(Kafka.Producer(bootstrapServers));
        services.AddHostedService<OutboxDispatcher>();
        services.AddHostedService<OrderConsumers>();
        
        return services;
    }
}