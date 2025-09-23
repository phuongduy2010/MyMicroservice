using OrderSvc.Application.Services.Commands;
using OrderSvc.Application.Services.Queries;
using Microsoft.Extensions.DependencyInjection;
namespace OrderSvc.Application;
public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PlaceOrderHandler>();
        services.AddScoped<GetOrderHandler>();
        return services;
    }
}