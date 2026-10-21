using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.HomeConfig;
using ByShutterBackend.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ByShutterBackend.Infrastructure.Repositories;

public class PortfolioTileRepository : IPortfolioTileRepository
{
    private readonly ByShutterDbContext _context;

    public PortfolioTileRepository(ByShutterDbContext context)
    {
        _context = context;
    }

    public async Task<PortfolioTile?> GetPortfolioTileAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PortfolioTiles
            .Include(pt => pt.Photo)
            .FirstOrDefaultAsync(pt => pt.Id == id, ct);
    }

    public async Task<PortfolioTile> CreatePortfolioTileAsync(PortfolioTile portfolioTile, CancellationToken ct = default)
    {
        await _context.PortfolioTiles.AddAsync(portfolioTile, ct);
        await _context.SaveChangesAsync(ct);
        
        return portfolioTile;
    }

    public async Task<PortfolioTile> UpdatePortfolioTileAsync(PortfolioTile portfolioTile, CancellationToken ct = default)
    {
        _context.PortfolioTiles.Update(portfolioTile);
        await _context.SaveChangesAsync(ct);
        
        return portfolioTile;
    }

    public async Task DeletePortfolioTileAsync(PortfolioTile portfolioTile, CancellationToken ct = default)
    {
        _context.PortfolioTiles.Remove(portfolioTile);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<PortfolioTile>> GetPortfolioTilesAsync(int skip = 0, int take = 4, CancellationToken ct = default)
    {
        return _context.PortfolioTiles
            .Include(pt => pt.Photo)
            .Skip(skip)
            .Take(take);
    }
}