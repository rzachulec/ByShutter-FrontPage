using MediatR;
using FluentResults;

namespace ByShutterBackend.Application.Portfolio.Commands.Albums;

public record RemoveImagesFromAlbumCommand(
    Guid AlbumId,
    IEnumerable<Guid> ImageIds
) : IRequest<Result>;