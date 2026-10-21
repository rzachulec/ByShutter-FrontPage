using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries.HomeProfile;

public record GetHeroSectionQuery(
    Guid HomeProfileId
) : IRequest<HeroSectionDto>;