using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public record RemovePortfolioTileFromProfileCommand(
    Guid HomeProfileId,
    Guid PortfolioTileId
) : IRequest<Result>;