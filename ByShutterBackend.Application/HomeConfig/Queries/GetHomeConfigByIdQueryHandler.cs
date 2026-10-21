using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.HomeConfig;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries;

public class GetHomeConfigByIdQueryHandler : IRequestHandler<GetHomeConfigByIdQuery, HomeProfileDto>
{
    private readonly IHomeProfileRepository _homeProfileRepository;

    public GetHomeConfigByIdQueryHandler(IHomeProfileRepository homeProfileRepository)
    {
        _homeProfileRepository = homeProfileRepository;
    }


    /// <summary>
    /// Access command for Home Profile.
    /// </summary>
    /// <param name="request">Record object with data to access the home profile</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Home Profile DTO</returns>
    /// <exception cref="NotFoundException">Thrown if any home profile wasn't found with given ID</exception>
    public async Task<HomeProfileDto> Handle(GetHomeConfigByIdQuery request, CancellationToken cancellationToken)
    {
        var homeProfile = await _homeProfileRepository.GetHomeProfileAsync(request.HomeConfigId, cancellationToken);

        if (homeProfile is null)
        {
            throw new NotFoundException(nameof(HomeProfile), request.HomeConfigId);
        }

        return new HomeProfileDto(
            homeProfile.Id,
            homeProfile.HeroMessage,
            homeProfile.Greetings,
            homeProfile.Quote,
            homeProfile.AboutMe,
            new PhotoDto(
                homeProfile.HeroImage.ServerPath,
                homeProfile.HeroImage.ImageAlt
            ),
            new PhotoDto(
                homeProfile.ProfileImage.ServerPath,
                homeProfile.ProfileImage.ImageAlt
            ),
            homeProfile.HeroImageId,
            homeProfile.ProfileImageId,
            homeProfile.PortfolioTiles.Select(tile => new PortfolioTileDto(
                tile.Id,
                tile.Name,
                tile.Description,
                new PhotoDto(tile.Photo.ServerPath, tile.Photo.ImageAlt),
                tile.ImageId
            ))
        );
    }
}