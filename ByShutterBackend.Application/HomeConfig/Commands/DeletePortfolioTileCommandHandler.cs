using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public class DeletePortfolioTileCommandHandler : IRequestHandler<DeletePortfolioTileCommand, Result>
{
    private readonly IPortfolioTileRepository _portfolioTileRepository;

    public DeletePortfolioTileCommandHandler(IPortfolioTileRepository portfolioTileRepository)
    {
        _portfolioTileRepository = portfolioTileRepository;
    }

    public async Task<Result> Handle(DeletePortfolioTileCommand request, CancellationToken cancellationToken)
    {
        var portfolioTile = await _portfolioTileRepository.GetPortfolioTileAsync(
            request.PortfolioTileId,
            cancellationToken
        );

        if (portfolioTile is null)
        {
            return Result.Ok();
        }

        await _portfolioTileRepository.DeletePortfolioTileAsync(portfolioTile, cancellationToken);

        return Result.Ok();
    }
}