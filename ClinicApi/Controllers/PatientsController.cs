using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicApi.Data;
using ClinicApi.Models;
using StackExchange.Redis;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly ClinicDbContext _db;
    private readonly IDatabase _cache;

    public PatientsController(ClinicDbContext db, IConnectionMultiplexer redis)
    {
        _db = db;
        _cache = redis.GetDatabase();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cached = await _cache.StringGetAsync("patients:all");
        if (cached.HasValue) return Content(cached.ToString(), "application/json");
        var list = await _db.Patients.Include(p => p.Doctor).ToListAsync();
        var json = System.Text.Json.JsonSerializer.Serialize(list,
            new System.Text.Json.JsonSerializerOptions {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles });
        await _cache.StringSetAsync("patients:all", json, TimeSpan.FromSeconds(60));
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var p = await _db.Patients.Include(x => x.Doctor).FirstOrDefaultAsync(x => x.Id == id);
        return p == null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Patient p)
    {
        _db.Patients.Add(p);
        await _db.SaveChangesAsync();
        await _cache.KeyDeleteAsync("patients:all");
        return CreatedAtAction(nameof(GetOne), new { id = p.Id }, p);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Patient p)
    {
        if (id != p.Id) return BadRequest();
        _db.Entry(p).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        await _cache.KeyDeleteAsync("patients:all");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Patients.FindAsync(id);
        if (p == null) return NotFound();
        _db.Patients.Remove(p);
        await _db.SaveChangesAsync();
        await _cache.KeyDeleteAsync("patients:all");
        return NoContent();
    }
}
