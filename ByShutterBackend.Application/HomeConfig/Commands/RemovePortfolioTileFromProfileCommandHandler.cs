using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public class RemovePortfolioTileFromProfileCommandHandler
    : IRequestHandler<RemovePortfolioTileFromProfileCommand, Result>
{
    private readonly IHomeProfileRepository _homeProfileRepository;

    public RemovePortfolioTileFromProfileCommandHandler(IHomeProfileRepository homeProfileRepository)
    {
        _homeProfileRepository = homeProfileRepository;
    }

    public async Task<Result> Handle(RemovePortfolioTileFromProfileCommand request,
        CancellationToken cancellationToken)
    {
        var homeProfile = await _homeProfileRepository.GetHomeProfileAsync(request.HomeProfileId, cancellationToken);

        if (homeProfile is null)
        {
            return Result.Fail($"HomeProfile with key {request.HomeProfileId} not found.");
        }

        homeProfile.RemovePortfolioTile(request.PortfolioTileId);

        await _homeProfileRepository.UpdateHomeProfileAsync(homeProfile, cancellationToken);

        return Result.Ok();
    }
}