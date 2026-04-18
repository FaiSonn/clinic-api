using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicApi.Data;
using ClinicApi.Models;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly ClinicDbContext _db;
    public AppointmentsController(ClinicDbContext db) { _db = db; }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Appointments.Include(a => a.Patient).Include(a => a.Doctor).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var a = await _db.Appointments.Include(x => x.Patient)
            .Include(x => x.Doctor).FirstOrDefaultAsync(x => x.Id == id);
        return a == null ? NotFound() : Ok(a);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Appointment a)
    {
        _db.Appointments.Add(a);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = a.Id }, a);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Appointment a)
    {
        if (id != a.Id) return BadRequest();
        _db.Entry(a).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.Appointments.FindAsync(id);
        if (a == null) return NotFound();
        _db.Appointments.Remove(a);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
