using ByShutterBackend.Application.HomeConfig.DTO;

namespace ByShutterBackend.Application.HomeConfig.Queries.HomeProfile;

public record HeroSectionDto(
    string HeroMessage,
    PhotoDto HeroImage
);