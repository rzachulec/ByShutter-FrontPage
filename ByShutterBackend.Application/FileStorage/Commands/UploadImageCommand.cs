using MediatR;

namespace ByShutterBackend.Application.FileStorage.Commands;

/// <summary>
/// Stores an upload: the untouched original (kept under the originals directory) and the
/// web-sized rendition that the site serves.
/// </summary>
public record UploadImageCommand(
    Stream Original,
    string OriginalName,
    byte[] WebImage,
    string WebName,
    string Directory
) : IRequest;
