using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public record DeletePortfolioTileCommand(
    Guid PortfolioTileId
) : IRequest<Result>;