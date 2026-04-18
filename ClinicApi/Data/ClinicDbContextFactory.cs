using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicApi.Data;

public class ClinicDbContextFactory : IDesignTimeDbContextFactory<ClinicDbContext>
{
    public ClinicDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ClinicDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=clinic;Username=postgres;Password=postgres");
        return new ClinicDbContext(optionsBuilder.Options);
    }
}
