namespace ClinicApi.Models;

public class Patient
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Phone { get; set; } = "";
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
