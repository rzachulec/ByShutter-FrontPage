using ByShutterBackend.Application.Portfolio.DTO;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Albums;

public record GetAlbumByIdQuery(
    Guid AlbumId
) : IRequest<AlbumDto>;