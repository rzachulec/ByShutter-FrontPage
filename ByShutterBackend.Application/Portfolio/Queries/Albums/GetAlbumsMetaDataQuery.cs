using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.DTO;

public record GetAlbumsMetaDataQuery(
    int PageSize,
    int Page
) : IRequest<Result<IEnumerable<AlbumMetaDataDto>>>;