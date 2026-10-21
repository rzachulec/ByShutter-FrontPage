using ByShutterBackend.Domain.HomeConfig;
using ByShutterBackend.Domain.Portfolio;
using ByShutterBackend.Domain.ClientAccess;
using Microsoft.EntityFrameworkCore;

namespace ByShutterBackend.Infrastructure.Persistance;

public class ByShutterDbContext : DbContext
{
    private const string DefaultSchema = "ByShutter";
    
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<AlbumPhoto> AlbumPhotos => Set<AlbumPhoto>();
    
    public DbSet<HomeProfile> HomeProfiles => Set<HomeProfile>();
    public DbSet<PortfolioTile> PortfolioTiles => Set<PortfolioTile>();
    public DbSet<User> Users => Set<User>();

    public ByShutterDbContext(DbContextOptions<ByShutterDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ByShutterDbContext).Assembly);
    }
}