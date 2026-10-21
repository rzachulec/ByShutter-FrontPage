namespace ByShutterBackend.Domain.Portfolio;

public class AlbumPhoto
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    public Guid AlbumId { get; private set; }
    public Album Album { get; init; }
    
    public Guid PhotoId { get; private set; }
    public Photo Photo { get; init; }
    
    public DateTime AddedToAlbumAt { get; private set; } = DateTime.UtcNow;

    public AlbumPhoto(Guid albumId, Guid photoId)
    {
        AlbumId = albumId;
        PhotoId = photoId;
    }
}