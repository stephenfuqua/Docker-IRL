using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
const string AllowedHostsPolicy = "AllowReactApp";

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        AllowedHostsPolicy,
        policy =>
        {
            _ = policy
                .WithOrigins(config.GetValue<string>("AllowedHosts") ?? "")
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services.AddHealthChecks()
    .AddCheck(
        "MSSQL-check",
        new api.SqlConnectionHealthCheck(config.GetConnectionString("DefaultConnection") ?? ""),
        HealthStatus.Unhealthy,
        ["mssql"]
    );

var app = builder.Build();
app.UsePathBase($"{config.GetValue<string>("AppPathBase") ?? "/"}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AllowedHostsPolicy);

if (config.GetValue<bool>("UseHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapHealthChecks("/health");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
