using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Infrastructure.Persistance;

namespace ByShutterBackend.Infrastructure.Repositories;

public class UnitOfWork : IDisposable, IAsyncDisposable
{
    private readonly ByShutterDbContext _context;
    
    private readonly IAlbumRepository? _albumRepository;
    private readonly IPhotoRepository? _photoRepository;
    
    public UnitOfWork(ByShutterDbContext context, IAlbumRepository? albumRepository, IPhotoRepository? photoRepository)
    {
        _context = context;
        _albumRepository = albumRepository;
        _photoRepository = photoRepository;
    }
    
    public async Task SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.SaveChangesAsync();
    }
}