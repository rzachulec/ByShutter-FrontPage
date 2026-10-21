using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ByShutterBackend.Infrastructure.Persistance;

public class ByShutterDbContextFactory : IDesignTimeDbContextFactory<ByShutterDbContext>
{
    public ByShutterDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ByShutterDbContext>();
        optionsBuilder.UseNpgsql("Host=postgres;Port=5432;Database=byshutter;Username=postgres;Password=byshutter_password;");
        
        return new ByShutterDbContext(optionsBuilder.Options);
    }
}