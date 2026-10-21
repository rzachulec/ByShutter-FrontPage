using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries.HomeProfile;

public class GetHomeImagesQueryHandler : IRequestHandler<GetHomeImagesQuery, HomeImagesDto>
{
    private readonly IHomeProfileRepository _homeProfileRepository;

    public GetHomeImagesQueryHandler(IHomeProfileRepository homeProfileRepository)
    {
        _homeProfileRepository = homeProfileRepository;
    }

    public async Task<HomeImagesDto> Handle(GetHomeImagesQuery request, CancellationToken cancellationToken)
    {
        var homeProfile = await _homeProfileRepository.GetHomeProfileAsync(request.HomeProfileId, cancellationToken);

        if (homeProfile is null)
        {
            throw new NotFoundException(nameof(HomeProfile), request.HomeProfileId);
        }

        return new HomeImagesDto(
            new PhotoDto(homeProfile.HeroImage.ServerPath, homeProfile.HeroImage.ImageAlt),
            new PhotoDto(homeProfile.ProfileImage.ServerPath, homeProfile.ProfileImage.ImageAlt)
        );
    }
}