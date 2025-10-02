using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Observability;

public static class ObservabilityExtensions
{
    public static void AddCTBOpenTelemetry(this WebApplicationBuilder builder, string serviceName, string otlpEndpoint)
    {
        var useHttp = otlpEndpoint.EndsWith(":4318", StringComparison.Ordinal);
        var protocol = useHttp ? OtlpExportProtocol.HttpProtobuf : OtlpExportProtocol.Grpc;
        var resource = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion: "1.0.0")
            .AddAttributes(new[]
            {
                new KeyValuePair<string, object>("service.namespace", "myMicroservices"),
                new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName),
            });

        // Logs
        // builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();
        builder.Logging.AddOpenTelemetry(o =>
        {
            o.SetResourceBuilder(resource);
            o.IncludeScopes = true;
            o.IncludeFormattedMessage = true;
            o.ParseStateValues = true;
            o.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(otlpEndpoint);
                    o.Protocol = protocol;
                });
        });

        // Traces + Metrics
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(_ => _.AddService(serviceName))
            .WithTracing(t => t
                .SetResourceBuilder(resource)
                .AddAspNetCoreInstrumentation(o => o.RecordException = true)
                .AddHttpClientInstrumentation()
                //  .AddSqlClientInstrumentation(o => o.SetDbStatementForText = true)
                // Add custom sources if you emit them:
                // .AddSource("eshop.orders", "eshop.payments", ...)
                .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(1.0))) // tune in prod
                .AddConsoleExporter()
                .AddOtlpExporter(o =>
                {
                     o.Endpoint = new Uri(otlpEndpoint);
                    o.Protocol = protocol;
                }))
            .WithMetrics(m => m
                .SetResourceBuilder(resource)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                //   .AddRuntimeInstrumentation()
                // .AddMeter("eshop.orders", "eshop.payments", ...)
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(otlpEndpoint);
                    o.Protocol = protocol;
                }));
    }
}
