using Microsoft.EntityFrameworkCore;
using ClinicApi.Data;
using ClinicApi.Models;
using StackExchange.Redis;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? "Host=localhost;Database=clinic;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ClinicDbContext>(opt => opt.UseNpgsql(connStr));

var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisUrl));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpMetrics();
app.MapMetrics();
app.MapControllers();
app.Run();
