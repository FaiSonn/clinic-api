namespace ClinicApi.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public DateTime DateTime { get; set; }
    public string Reason { get; set; } = "";
    public string Status { get; set; } = "pending";
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
