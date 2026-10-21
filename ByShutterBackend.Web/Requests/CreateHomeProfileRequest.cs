namespace ByShutterBackend.Web.Requests;

public record CreateHomeProfileRequest(
    string HeroMessage,
    string Greetings,
    string Quote,
    string AboutMe,
    Guid HeroImageId,
    Guid ProfileImageId
);