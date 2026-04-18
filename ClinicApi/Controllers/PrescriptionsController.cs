using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicApi.Data;
using ClinicApi.Models;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly ClinicDbContext _db;
    public PrescriptionsController(ClinicDbContext db) { _db = db; }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Prescriptions.Include(p => p.Patient).Include(p => p.Appointment).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var p = await _db.Prescriptions.FindAsync(id);
        return p == null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Prescription p)
    {
        _db.Prescriptions.Add(p);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = p.Id }, p);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Prescriptions.FindAsync(id);
        if (p == null) return NotFound();
        _db.Prescriptions.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
