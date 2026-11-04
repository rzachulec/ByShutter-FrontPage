using MediatR;

namespace ByShutterBackend.Application.FileStorage.Commands;

/// <summary>Produces the web-sized WebP rendition of an uploaded image.</summary>
public record OptimizeImageCommand(Stream Source) : IRequest<byte[]>;
