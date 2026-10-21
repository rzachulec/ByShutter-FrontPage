using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public record CreatePortfolioTileCommand(
    string Name,
    Guid ImageId,
    string? Description
) : IRequest<Result<Guid>>;