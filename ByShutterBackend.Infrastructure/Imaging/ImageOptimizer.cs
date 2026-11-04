using ByShutterBackend.Domain.Common.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ByShutterBackend.Infrastructure.Imaging;

public sealed class ImageOptimizer : IImageOptimizer
{
    /// <summary>Longest edge of the web rendition. Enough for a full-bleed hero on a 1080p/1440p screen.</summary>
    private const int MaxLongEdge = 2048;

    /// <summary>Lossy WebP quality; visually indistinguishable from the source at screen sizes.</summary>
    private const int Quality = 80;

    public async Task<byte[]> CreateWebRenditionAsync(Stream source, CancellationToken ct = default)
    {
        using var image = await Image.LoadAsync(source, ct);

        // Bake the EXIF orientation into the pixels first, because the metadata is dropped below.
        image.Mutate(ctx => ctx.AutoOrient());

        // Never upscale: small images are only re-encoded.
        if (Math.Max(image.Width, image.Height) > MaxLongEdge)
        {
            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(MaxLongEdge, MaxLongEdge),
                Mode = ResizeMode.Max,
            }));
        }

        // The web copy carries no camera/GPS/XMP metadata (the untouched original keeps it all).
        // The ICC colour profile is deliberately kept so wide-gamut photos still render correctly.
        image.Metadata.ExifProfile = null;
        image.Metadata.IptcProfile = null;
        image.Metadata.XmpProfile = null;

        using var output = new MemoryStream();
        await image.SaveAsync(output, new WebpEncoder { Quality = Quality, FileFormat = WebpFileFormatType.Lossy }, ct);
        return output.ToArray();
    }
}
