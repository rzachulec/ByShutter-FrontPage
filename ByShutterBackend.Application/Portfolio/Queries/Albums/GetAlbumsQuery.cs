using ByShutterBackend.Application.Portfolio.DTO;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Albums;

public record GetAlbumsQuery(
    int PageSize,
    int Page
) : IRequest<IEnumerable<AlbumDto>>;