using ByShutterBackend.Domain.Portfolio;

namespace ByShutterBackend.Domain.Common.Interfaces;

public interface IPhotoRepository
{
    Task<Photo?> GetPhotoAsync(Guid id, CancellationToken ct = default);
    Task<Photo?> GetPhotoByFileNameAsync(string fileName, CancellationToken ct = default);
    Task<Photo> CreatePhotoAsync(Photo photo, CancellationToken ct = default);
    Task<Photo> UpdatePhotoAsync(Photo photo, CancellationToken ct = default);
    Task DeletePhotoAsync(Photo photo, CancellationToken ct = default);
    Task<IEnumerable<Photo>> GetPhotosAsync(uint skip = 0, int take = 100, CancellationToken ct = default);

    Task<IEnumerable<Photo>> GetPhotosNotInAlbumAsync(Guid albumId, int skip = 0, int take = 100, CancellationToken ct = default);
}