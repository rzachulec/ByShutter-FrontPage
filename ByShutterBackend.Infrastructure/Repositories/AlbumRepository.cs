using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
using ByShutterBackend.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ByShutterBackend.Infrastructure.Repositories;

public class AlbumRepository : IAlbumRepository
{
    private readonly ByShutterDbContext _context;

    public AlbumRepository(ByShutterDbContext context)
    {
        _context = context;
    }

    public async Task<Album?> GetAlbumAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Albums
            .Include(album => album.Photos)
            .ThenInclude(p => p.Photo)
            .FirstOrDefaultAsync(album => album.Id == id, ct);
    }

    public async Task<Album> CreateAlbumAsync(Album album, CancellationToken ct = default)
    {
        _context.Albums.Add(album);
        await _context.SaveChangesAsync(ct);
        
        return album;
    }

    public async Task<Album> UpdateAlbumAsync(Album album, CancellationToken ct = default)
    {
        _context.Albums.Update(album);
        await _context.SaveChangesAsync(ct);
        
        return album;
    }

    public async Task DeleteAlbumAsync(Album album, CancellationToken ct = default)
    {
        _context.Albums.Remove(album);
        await _context.SaveChangesAsync(ct);
    }
    
    public async Task<Album?> GetAlbumByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Albums
            .Include(album => album.Photos)
            .ThenInclude(p => p.Photo)
            .FirstOrDefaultAsync(album => album.Name == name, ct);
    }

    public async Task<IEnumerable<Album>> GetAlbumsAsync(int skip = 0, int take = 10, CancellationToken ct = default)
    {
        return await _context.Albums
            .OrderBy(album => album.Id)
            .Include(album => album.Photos)
            .ThenInclude(p => p.Photo)
            .Skip(skip).Take(take).ToListAsync(ct);
    }

    public async Task<IEnumerable<AlbumPhoto>> GetAlbumPhotosAsync(Guid albumId, int skip = 0, int take = 10, CancellationToken ct = default)
    {
        var album = await GetAlbumAsync(albumId, ct);

        return album is null ? 
            [] : 
            album.Photos
                .Skip(skip)
                .Take(take);
    }
}