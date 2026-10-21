using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries;

public class GetHomeProfilesQueryHandler : IRequestHandler<GetHomeProfilesQuery, IEnumerable<HomeProfileDto>>
{
    private readonly IHomeProfileRepository _homeProfileRepository;

    public GetHomeProfilesQueryHandler(IHomeProfileRepository homeProfileRepository)
    {
        _homeProfileRepository = homeProfileRepository;
    }

    public async Task<IEnumerable<HomeProfileDto>> Handle(GetHomeProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var profiles = await _homeProfileRepository.GetHomeProfilesAsync(
            request.Page * request.PageSize, request.PageSize, cancellationToken);

        return profiles.Select(profile => new HomeProfileDto(
            profile.Id,
            profile.HeroMessage,
            profile.Greetings,
            profile.Quote,
            profile.AboutMe,
            new PhotoDto(profile.HeroImage.ServerPath, profile.HeroImage.ImageAlt),
            new PhotoDto(profile.ProfileImage.ServerPath, profile.ProfileImage.ImageAlt),
            profile.HeroImageId,
            profile.ProfileImageId,
            profile.PortfolioTiles.Select(tile => new PortfolioTileDto(
                tile.Id,
                tile.Name,
                tile.Description,
                new PhotoDto(tile.Photo.ServerPath, tile.Photo.ImageAlt),
                tile.ImageId
            ))
        ));
    }
}