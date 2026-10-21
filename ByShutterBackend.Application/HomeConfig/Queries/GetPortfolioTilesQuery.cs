using ByShutterBackend.Application.HomeConfig.DTO;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries;

public record GetPortfolioTilesQuery(
    int PageSize,
    int Page
) : IRequest<IEnumerable<PortfolioTileDto>>;