using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.HomeConfig;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries;

public class GetPortfolioTileByIdQueryHandler : IRequestHandler<GetPortfolioTileByIdQuery, PortfolioTileDto>
{
    private readonly IPortfolioTileRepository _portfolioTileRepository;

    public GetPortfolioTileByIdQueryHandler(IPortfolioTileRepository portfolioTileRepository)
    {
        _portfolioTileRepository = portfolioTileRepository;
    }

    public async Task<PortfolioTileDto> Handle(GetPortfolioTileByIdQuery request, CancellationToken cancellationToken)
    {
        var portfolioTile =
            await _portfolioTileRepository.GetPortfolioTileAsync(request.PortfolioTileId, cancellationToken);

        if (portfolioTile is null)
        {
            throw new NotFoundException(nameof(PortfolioTile), request.PortfolioTileId);
        }

        return new PortfolioTileDto(
            portfolioTile.Id,
            portfolioTile.Name,
            portfolioTile.Description,
            new PhotoDto(
                portfolioTile.Photo.ServerPath,
                portfolioTile.Photo.ImageAlt
            ),
            portfolioTile.ImageId
        );
    }
}