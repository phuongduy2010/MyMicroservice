using Microsoft.Extensions.DependencyInjection;
using NotificationSvc.Infrastructure.Messaging;
using Shared;

namespace NotificationSvc.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
         this IServiceCollection services,
        string bootstrapServers)
    {
        services.AddHostedService<NotificationConsumer>();
        services.AddSingleton(Kafka.Producer(bootstrapServers));
        return services;
    }
}