using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public record AssignPortfolioTileToProfileCommand(
    Guid PortfolioTileId,
    Guid HomeProfileId
) : IRequest<Result>;