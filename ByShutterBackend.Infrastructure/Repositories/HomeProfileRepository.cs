using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.HomeConfig;
using ByShutterBackend.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ByShutterBackend.Infrastructure.Repositories;

public class HomeProfileRepository : IHomeProfileRepository
{
    private readonly ByShutterDbContext _context;

    public HomeProfileRepository(ByShutterDbContext context)
    {
        _context = context;
    }

    public async Task<HomeProfile?> GetHomeProfileAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.HomeProfiles
            .Include(hp => hp.PortfolioTiles)
            .ThenInclude(tile => tile.Photo)
            .Include(hp => hp.HeroImage)
            .Include(hp => hp.ProfileImage)
            .FirstOrDefaultAsync(hp => hp.Id == id, ct);
    }

    public async Task<HomeProfile> CreateHomeProfileAsync(HomeProfile homeProfile, CancellationToken ct = default)
    {
        await _context.HomeProfiles.AddAsync(homeProfile, ct);
        await _context.SaveChangesAsync(ct);
        
        return homeProfile;
    }

    public async Task<HomeProfile> UpdateHomeProfileAsync(HomeProfile homeProfile, CancellationToken ct = default)
    {
        _context.HomeProfiles.Update(homeProfile);
        await _context.SaveChangesAsync(ct);
        
        return homeProfile;
    }

    public async Task DeleteHomeProfileAsync(HomeProfile homeProfile, CancellationToken ct = default)
    {
        _context.HomeProfiles.Remove(homeProfile);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<HomeProfile>> GetHomeProfilesAsync(int skip = 0, int take = 10,
        CancellationToken ct = default)
    {
        return await _context.HomeProfiles
            .Include(hp => hp.PortfolioTiles)
            .ThenInclude(tile => tile.Photo)
            .Include(hp => hp.HeroImage)
            .Include(hp => hp.ProfileImage)
            .OrderBy(hp => hp.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }
}