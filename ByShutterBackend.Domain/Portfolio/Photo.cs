namespace ByShutterBackend.Domain.Portfolio;

public class Photo
{
    /// <summary>Sub-directory of the image store that keeps the untouched uploads.</summary>
    public const string OriginalsDirectory = "originals";

    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>Name of the web-sized file that is shown on the site.</summary>
    public string FileName { get; set; }

    /// <summary>
    /// Name of the untouched upload, stored under <see cref="OriginalsDirectory"/>.
    /// Null for photos uploaded before originals were kept.
    /// </summary>
    public string? OriginalFileName { get; set; }

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