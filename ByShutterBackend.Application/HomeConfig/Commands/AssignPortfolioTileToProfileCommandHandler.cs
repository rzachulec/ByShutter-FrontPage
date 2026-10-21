using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public class AssignPortfolioTileToProfileCommandHandler : IRequestHandler<AssignPortfolioTileToProfileCommand, Result>
{
    private readonly IPortfolioTileRepository _portfolioTileRepository;
    private readonly IHomeProfileRepository _homeProfileRepository;

    public AssignPortfolioTileToProfileCommandHandler(IPortfolioTileRepository portfolioTileRepository,
        IHomeProfileRepository homeProfileRepository)
    {
        _portfolioTileRepository = portfolioTileRepository;
        _homeProfileRepository = homeProfileRepository;
    }

    public async Task<Result> Handle(AssignPortfolioTileToProfileCommand request, CancellationToken cancellationToken)
    {
        var homeProfile = await _homeProfileRepository.GetHomeProfileAsync(request.HomeProfileId, cancellationToken);

        if (homeProfile is null)
        {
            return Result.Fail($"HomeProfile with key {request.HomeProfileId} not found.");
        }
        
        var portfolioTile =
            await _portfolioTileRepository.GetPortfolioTileAsync(request.PortfolioTileId, cancellationToken);
        
        if (portfolioTile is null)
        {
            return Result.Fail($"PortfolioTile with key {request.PortfolioTileId} not found.");
        }
        
        homeProfile.AddPortfolioTile(portfolioTile);
        
        // TODO: Save with UnitOfWork
        await _portfolioTileRepository.UpdatePortfolioTileAsync(portfolioTile, cancellationToken);

        return Result.Ok();
    }
}