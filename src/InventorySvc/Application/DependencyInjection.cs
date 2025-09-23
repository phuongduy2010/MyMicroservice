
using InventorySvc.Application.Services;
using InventorySvc.Application.Services.Commands;
using InventorySvc.Application.Services.Queries;
using Microsoft.Extensions.DependencyInjection;
namespace InventorySvc.Application;
public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ConsumeOrderCreated>();
        services.AddScoped<PlaceInventoryHandler>();
        services.AddScoped<GetInventoryHandler>();
        return services;
    }
}