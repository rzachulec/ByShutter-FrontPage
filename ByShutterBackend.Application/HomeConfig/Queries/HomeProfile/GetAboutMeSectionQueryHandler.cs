using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries.HomeProfile;

public class GetAboutMeSectionQueryHandler : IRequestHandler<GetAboutMeSectionQuery, AboutMeSectionDto>
{
    private readonly IHomeProfileRepository _homeProfileRepository;

    public GetAboutMeSectionQueryHandler(IHomeProfileRepository homeProfileRepository)
    {
        _homeProfileRepository = homeProfileRepository;
    }

    public async Task<AboutMeSectionDto> Handle(GetAboutMeSectionQuery request, CancellationToken cancellationToken)
    {
        var homeProfile = await _homeProfileRepository.GetHomeProfileAsync(request.HomeProfileId, cancellationToken);

        if (homeProfile is null)
        {
            throw new NotFoundException(nameof(HomeProfile), request.HomeProfileId);
        }

        return new AboutMeSectionDto(
            homeProfile.Greetings,
            homeProfile.Quote,
            homeProfile.AboutMe,
            new PhotoDto(
                homeProfile.ProfileImage.ServerPath,
                homeProfile.ProfileImage.ImageAlt)
        );
    }
}