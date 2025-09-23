
using NotificationSvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var bootstrap = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
builder.Services.AddInfrastructure(bootstrap);
var app = builder.Build();
app.MapGet("/health", () => "ok");
app.Run();
