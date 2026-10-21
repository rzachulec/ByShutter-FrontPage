namespace ByShutterBackend.Domain.Portfolio;

public class Photo
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public string FileName { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime UploadedAt { get; init; } = DateTime.UtcNow;

    public string ServerPath { get; set; }
    
    public string ImageAlt { get; set; }

    public Photo(string fileName, string serverPath, string imageAlt)
    {
        FileName = fileName;
        ServerPath = serverPath;
        ImageAlt = imageAlt;
    }
}