using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries;

public class GetPortfolioTilesQueryHandler : IRequestHandler<GetPortfolioTilesQuery, IEnumerable<PortfolioTileDto>>
{
    private readonly IPortfolioTileRepository _portfolioTileRepository;

    public GetPortfolioTilesQueryHandler(IPortfolioTileRepository portfolioTileRepository)
    {
        _portfolioTileRepository = portfolioTileRepository;
    }

    public async Task<IEnumerable<PortfolioTileDto>> Handle(GetPortfolioTilesQuery request,
        CancellationToken cancellationToken)
    {
        var skip = request.Page * request.PageSize;
        var take = request.PageSize;

        var portfolioTiles = await _portfolioTileRepository.GetPortfolioTilesAsync(skip, take, cancellationToken);

        return portfolioTiles.Select(pt =>
            new PortfolioTileDto(
                pt.Id,
                pt.Name,
                pt.Description,
                new PhotoDto(
                    pt.Photo.ServerPath,
                    pt.Photo.ImageAlt
                ),
                pt.ImageId
            )
        );
    }
}