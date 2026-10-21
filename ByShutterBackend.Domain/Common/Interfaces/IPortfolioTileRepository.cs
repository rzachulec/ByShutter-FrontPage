using ByShutterBackend.Domain.HomeConfig;

namespace ByShutterBackend.Domain.Common.Interfaces;

public interface IPortfolioTileRepository
{
    public Task<PortfolioTile?> GetPortfolioTileAsync(Guid id, CancellationToken ct = default);
    
    public Task<PortfolioTile> CreatePortfolioTileAsync(PortfolioTile portfolioTile, CancellationToken ct = default);
    
    public Task<PortfolioTile> UpdatePortfolioTileAsync(PortfolioTile portfolioTile, CancellationToken ct = default);
 
    public Task DeletePortfolioTileAsync(PortfolioTile portfolioTile, CancellationToken ct = default);

    public Task<IEnumerable<PortfolioTile>> GetPortfolioTilesAsync(int skip = 0, int take = 4,
        CancellationToken ct = default);
}