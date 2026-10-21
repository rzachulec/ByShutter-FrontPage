using ByShutterBackend.Application.HomeConfig.DTO;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries.HomeProfile;

public record GetAboutMeSectionQuery(
    Guid HomeProfileId
) : IRequest<AboutMeSectionDto>;