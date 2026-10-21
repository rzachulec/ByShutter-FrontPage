using ByShutterBackend.Domain.Common.Exceptions;

namespace ByShutterBackend.Domain.Portfolio;

public class Album
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    public string Name { get; private set; }
    public string Description { get; private set; }

    public ICollection<AlbumPhoto> Photos { get; private set; } = [];

    public Album(string name, string description = "")
    {
        Name = name;
        Description = description;
    }

    public void AddPhoto(AlbumPhoto photo)
    {
        if (Photos.Any(p => p.Id == photo.Id))
        {
            throw new DuplicationException(photo, nameof(Photos));
        }
        
        Photos.Add(photo);
    }

    public void RemovePhotos(IEnumerable<Guid> photoIds)
    {
        var ids = photoIds.ToHashSet();

        Photos = Photos
            .Where(photo => !ids.Contains(photo.PhotoId))
            .ToList();
    }
}