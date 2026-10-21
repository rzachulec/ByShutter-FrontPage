using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
using ByShutterBackend.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ByShutterBackend.Infrastructure.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly ByShutterDbContext _context;

    public PhotoRepository(ByShutterDbContext context)
    {
        _context = context;
    }

    public async Task<Photo?> GetPhotoAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Photos.AsNoTracking().FirstOrDefaultAsync(photo => photo.Id == id, ct);
    }

    public async Task<Photo?> GetPhotoByFileNameAsync(string fileName, CancellationToken ct = default)
    {
        return await _context.Photos.AsNoTracking().FirstOrDefaultAsync(photo => photo.FileName == fileName, ct);
    }

    public async Task<Photo> CreatePhotoAsync(Photo photo, CancellationToken ct = default)
    {
        _context.Photos.Add(photo);
        await _context.SaveChangesAsync(ct);
        
        return photo;
    }

    public async Task<Photo> UpdatePhotoAsync(Photo photo, CancellationToken ct = default)
    {
        _context.Photos.Update(photo);
        await _context.SaveChangesAsync(ct);
        
        return photo;
    }

    public async Task DeletePhotoAsync(Photo photo, CancellationToken ct = default)
    {
        _context.Photos.Remove(photo);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Photo>> GetPhotosAsync(uint skip = 0, int take = 100, CancellationToken ct = default)
    {
        return await _context.Photos
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Skip((int)skip).Take(take).ToListAsync(ct);
    }

    public async Task<IEnumerable<Photo>> GetPhotosNotInAlbumAsync(Guid albumId, int skip = 0, int take = 100, CancellationToken ct = default)
    {
        // Budujemy zapytanie jako IQueryable (podzapytanie SQL)
        var albumPhotoIds = _context.Albums
            .Where(a => a.Id == albumId)
            .SelectMany(a => a.Photos.Select(ap => ap.PhotoId));

        // EF Core połączy to w JEDNO zapytanie SQL: SELECT ... WHERE id NOT IN (SELECT photo_id ...)
        return await _context.Photos
            .AsNoTracking()
            .Where(p => !albumPhotoIds.Contains(p.Id))
            .OrderBy(p => p.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }
}