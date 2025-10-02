using OrderSvc.Api.Middlewares;
using OrderSvc.Application;
using OrderSvc.Infrastructure;
var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("db") ?? "Host=localhost;Port=5432;Database=order_db;Username=postgres;Password=postgres";
var bootstrap = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddInfrastructure(cs, bootstrap);
builder.Services.AddApplication();
builder.Services.AddControllers();
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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("ok"));
app.Run();

