using ByShutterBackend.Application.Portfolio.DTO;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Albums;

public record GetAlbumPhotosQuery(
    Guid AlbumId,
    int Skip,
    int Take
) : IRequest<Result<IEnumerable<PhotoDto>>>;