using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public record UpdatePortfolioTileCommand(
    Guid PortfolioTileId,
    string Name,
    Guid ImageId,
    string? Description
) : IRequest<Result>;