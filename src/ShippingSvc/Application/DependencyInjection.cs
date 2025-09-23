using Microsoft.Extensions.DependencyInjection;
using ShippingSvc.Application.Services.Commands;
using ShippingSvc.Application.Services.Queries;
namespace ShippingSvc.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PlaceShippingHandler>();
        services.AddScoped<GetShippingHandler>();
        return services;
    }
}