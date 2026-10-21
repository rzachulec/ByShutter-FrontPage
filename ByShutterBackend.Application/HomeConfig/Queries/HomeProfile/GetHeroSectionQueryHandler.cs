using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Application.HomeConfig.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries.HomeProfile;

public class GetHeroSectionQueryHandler : IRequestHandler<GetHeroSectionQuery, HeroSectionDto>
{
    private readonly IHomeProfileRepository _homeProfileRepository;

    public GetHeroSectionQueryHandler(IHomeProfileRepository homeProfileRepository)
    {
        _homeProfileRepository = homeProfileRepository;
    }

    public async Task<HeroSectionDto> Handle(GetHeroSectionQuery request, CancellationToken cancellationToken)
    {
        var homeProfile = await _homeProfileRepository.GetHomeProfileAsync(request.HomeProfileId, cancellationToken);

        if (homeProfile is null)
        {
            throw new NotFoundException(nameof(HomeProfile), request.HomeProfileId);
        }

        return new HeroSectionDto(
            homeProfile.HeroMessage,
            new PhotoDto(
                homeProfile.HeroImage.ServerPath,
                homeProfile.HeroImage.ImageAlt
            )
        );
    }
}