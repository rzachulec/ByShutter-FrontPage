namespace ByShutterBackend.Application.HomeConfig.DTO;

public record AboutMeSectionDto(
    string Greetings,
    string Quote,
    string AboutMe,
    PhotoDto ProfileImage
);