namespace ByShutterBackend.Application.HomeConfig.DTO;

public record HomeProfileDto(
    Guid Id,
    string HeroMessage,
    string Greetings,
    string Quote,
    string AboutMe,
    PhotoDto HeroImage,
    PhotoDto ProfileImage,
    Guid HeroImageId,
    Guid ProfileImageId,
    IEnumerable<PortfolioTileDto> PortfolioTiles
);