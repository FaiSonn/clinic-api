namespace ClinicApi.Models;

public class Prescription
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public string Drug { get; set; } = "";
    public string Dosage { get; set; } = "";
    public int DurationDays { get; set; }
}
