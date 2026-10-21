using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Queries.HomeProfile;

public record GetHomeImagesQuery(
    Guid HomeProfileId
) : IRequest<HomeImagesDto>;