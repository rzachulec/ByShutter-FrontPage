using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.HomeConfig;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public class UpdatePortfolioTileCommandHandler : IRequestHandler<UpdatePortfolioTileCommand, Result>
{
    private readonly IPortfolioTileRepository _portfolioTileRepository;

    public UpdatePortfolioTileCommandHandler(IPortfolioTileRepository portfolioTileRepository)
    {
        _portfolioTileRepository = portfolioTileRepository;
    }

    public async Task<Result> Handle(UpdatePortfolioTileCommand request, CancellationToken cancellationToken)
    {
        var portfolioTile = await _portfolioTileRepository.GetPortfolioTileAsync(
            request.PortfolioTileId,
            cancellationToken
        );

        if (portfolioTile is null)
        {
            return Result.Fail($"PortfolioTile with key {request.PortfolioTileId} not found.");
        }

        portfolioTile.Name = request.Name;
        portfolioTile.ImageId = request.ImageId;
        portfolioTile.Description = request.Description;

        await _portfolioTileRepository.UpdatePortfolioTileAsync(portfolioTile, cancellationToken);

        return Result.Ok();
    }
}