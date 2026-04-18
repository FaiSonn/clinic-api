using Microsoft.EntityFrameworkCore;
using ClinicApi.Models;

namespace ClinicApi.Data;

public class ClinicDbContext : DbContext
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor { Id=1, FullName="Петров Алексей Васильевич",
                Specialization="Терапевт", Room="101", Schedule="Пн-Пт 9:00-15:00" },
            new Doctor { Id=2, FullName="Козлова Наталья Сергеевна",
                Specialization="Кардиолог", Room="205", Schedule="Вт,Чт 10:00-16:00" }
        );
        modelBuilder.Entity<Patient>().HasData(
            new Patient { Id=1, FullName="Иванова Мария Петровна",
                DateOfBirth=new DateTime(1985,3,12),
                Phone="+7 912 345-67-89", DoctorId=1 }
        );
    }
}
