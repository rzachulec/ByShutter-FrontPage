using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.HomeConfig;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public class CreatePortfolioTileCommandHandler : IRequestHandler<CreatePortfolioTileCommand, Result<Guid>>
{
    private readonly IPortfolioTileRepository _portfolioTileRepository;

    public CreatePortfolioTileCommandHandler(IPortfolioTileRepository portfolioTileRepository)
    {
        _portfolioTileRepository = portfolioTileRepository;
    }

    public async Task<Result<Guid>> Handle(CreatePortfolioTileCommand request, CancellationToken cancellationToken)
    {
        var portfolioTile = new PortfolioTile(
            request.Name,
            request.Description,
            request.ImageId
        );
        
        await _portfolioTileRepository.CreatePortfolioTileAsync(portfolioTile, cancellationToken);

        return Result.Ok(portfolioTile.Id);
    }
}