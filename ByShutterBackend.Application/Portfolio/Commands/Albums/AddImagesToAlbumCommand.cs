using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands.Albums;

public record AddImagesToAlbumCommand(
    Guid AlbumId,
    IEnumerable<Guid> ImageIds
) : IRequest;