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

    if (!db.Doctors.Any())
    {
        db.Doctors.AddRange(
            new Doctor { FullName="Петров Алексей Васильевич",
                Specialization="Терапевт", Room="101", Schedule="Пн-Пт 9:00-15:00" },
            new Doctor { FullName="Козлова Наталья Сергеевна",
                Specialization="Кардиолог", Room="205", Schedule="Вт,Чт 10:00-16:00" }
        );
        db.SaveChanges();
    }

    if (!db.Patients.Any())
    {
        db.Patients.AddRange(
            new Patient { FullName="Иванова Мария Петровна",
                DateOfBirth=new DateTime(1985,3,12),
                Phone="+7 912 345-67-89", DoctorId=1 }
        );
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpMetrics();
app.MapMetrics();
app.MapControllers();
app.Run();
