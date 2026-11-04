namespace ByShutterBackend.Domain.Common.Interfaces;

public interface IImageOptimizer
{
    /// <summary>
    /// Decodes an uploaded image and returns a web-sized WebP rendition of it.
    /// Throws if <paramref name="source"/> is not a decodable image.
    /// </summary>
    Task<byte[]> CreateWebRenditionAsync(Stream source, CancellationToken ct = default);
}
