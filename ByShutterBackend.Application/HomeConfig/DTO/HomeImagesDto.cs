using ByShutterBackend.Application.HomeConfig.DTO;

namespace ByShutterBackend.Application.HomeConfig.Queries.HomeProfile;

public record HomeImagesDto(
    PhotoDto HeroImage,
    PhotoDto ProfileImage
);