using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicApi.Data;
using ClinicApi.Models;
using StackExchange.Redis;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly ClinicDbContext _db;
    private readonly IDatabase _cache;

    public DoctorsController(ClinicDbContext db, IConnectionMultiplexer redis)
    {
        _db = db;
        _cache = redis.GetDatabase();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cached = await _cache.StringGetAsync("doctors:all");
        if (cached.HasValue) return Content(cached.ToString(), "application/json");
        var list = await _db.Doctors.ToListAsync();
        var json = System.Text.Json.JsonSerializer.Serialize(list);
        await _cache.StringSetAsync("doctors:all", json, TimeSpan.FromSeconds(60));
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var d = await _db.Doctors.FindAsync(id);
        return d == null ? NotFound() : Ok(d);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Doctor d)
    {
        _db.Doctors.Add(d);
        await _db.SaveChangesAsync();
        await _cache.KeyDeleteAsync("doctors:all");
        return CreatedAtAction(nameof(GetOne), new { id = d.Id }, d);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Doctor d)
    {
        if (id != d.Id) return BadRequest();
        _db.Entry(d).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        await _cache.KeyDeleteAsync("doctors:all");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var d = await _db.Doctors.FindAsync(id);
        if (d == null) return NotFound();
        _db.Doctors.Remove(d);
        await _db.SaveChangesAsync();
        await _cache.KeyDeleteAsync("doctors:all");
        return NoContent();
    }
}
