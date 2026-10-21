using ByShutterBackend.Application.HomeConfig.DTO;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries;

public record GetPortfolioTileByIdQuery(
    Guid PortfolioTileId
) : IRequest<PortfolioTileDto>;