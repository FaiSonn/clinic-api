namespace ClinicApi.Models;

public class Doctor
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Specialization { get; set; } = "";
    public string Room { get; set; } = "";
    public string Schedule { get; set; } = "";
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
