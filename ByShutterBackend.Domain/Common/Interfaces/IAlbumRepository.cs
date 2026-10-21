using ByShutterBackend.Domain.Portfolio;

namespace ByShutterBackend.Domain.Common.Interfaces;

public interface IAlbumRepository
{
    Task<Album?> GetAlbumAsync(Guid id, CancellationToken ct = default);
    
    Task<Album> CreateAlbumAsync(Album album, CancellationToken ct = default);
    
    Task<Album> UpdateAlbumAsync(Album album, CancellationToken ct = default);
    
    Task DeleteAlbumAsync(Album album, CancellationToken ct = default);

    Task<Album?> GetAlbumByNameAsync(string name, CancellationToken ct = default);
    
    Task<IEnumerable<Album>> GetAlbumsAsync(int skip = 0, int take = 10, CancellationToken ct = default);

    Task<IEnumerable<AlbumPhoto>> GetAlbumPhotosAsync(Guid albumId, int skip = 0, int take = 10, CancellationToken ct = default);
}