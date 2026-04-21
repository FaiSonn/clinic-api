using Microsoft.EntityFrameworkCore;
using ClinicApi.Data;
using ClinicApi.Models;
using StackExchange.Redis;
using Prometheus;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? "Host=localhost;Database=clinic;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ClinicDbContext>(opt => opt.UseNpgsql(connStr));

var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisUrl));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    opts.JsonSerializerOptions.WriteIndented = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpMetrics();
app.MapMetrics();
app.MapControllers();
app.Run();
