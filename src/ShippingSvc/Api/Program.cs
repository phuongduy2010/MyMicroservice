
using ShippingSvc.Application;
using ShippingSvc.Infrastructure;
using Shared.Observability;
var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("db") ?? "Host=localhost;Port=5432;Database=shipping_db;Username=postgres;Password=postgres";
var bootstrap = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
builder.Services.AddInfrastructure(cs, bootstrap);
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.AddCTBOpenTelemetry("shippingSvc", "http://localhost:4317");
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // add swagger ui
    app.UseSwaggerUi(option => 
    {
        option.DocumentPath = "/openapi/v1.json";
    });
}
app.MapGet("/health", () => "ok");
app.MapControllers();
app.Run();
