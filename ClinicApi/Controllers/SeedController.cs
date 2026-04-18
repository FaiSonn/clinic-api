using Microsoft.AspNetCore.Mvc;
using ClinicApi.Data;
using ClinicApi.Models;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly ClinicDbContext _db;
    public SeedController(ClinicDbContext db) { _db = db; }

    [HttpPost]
    public IActionResult Seed()
    {
        if (!_db.Doctors.Any())
        {
            _db.Doctors.Add(new Doctor {
                FullName = "Петров Алексей Васильевич",
                Specialization = "Терапевт",
                Room = "101",
                Schedule = "Пн-Пт 9:00-15:00"
            });
            _db.Doctors.Add(new Doctor {
                FullName = "Козлова Наталья Сергеевна",
                Specialization = "Кардиолог",
                Room = "205",
                Schedule = "Вт,Чт 10:00-16:00"
            });
            _db.SaveChanges();
        }

        if (!_db.Patients.Any())
        {
            _db.Patients.Add(new Patient {
                FullName = "Иванова Мария Петровна",
                DateOfBirth = new DateTime(1985, 3, 12, 0, 0, 0, DateTimeKind.Utc),
                Phone = "+7 912 345-67-89",
                DoctorId = 1
            });
            _db.SaveChanges();
        }

        return Ok(new {
            doctors = _db.Doctors.Count(),
            patients = _db.Patients.Count()
        });
    }
}
