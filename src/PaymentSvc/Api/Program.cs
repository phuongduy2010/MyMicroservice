
using PaymentSvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("db") ?? "Host=localhost;Port=5432;Database=payment_db;Username=postgres;Password=postgres";
var bootstrap = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
builder.Services.AddInfrastructure(cs, bootstrap);
var app = builder.Build();
app.MapGet("/health", () => "ok");
app.Run();
