namespace ByShutterBackend.Web.Requests;

public record UpdateHomeProfileRequest(
    string HeroMessage,
    string Greetings,
    string Quote,
    string AboutMe,
    Guid HeroImageId,
    Guid ProfileImageId
);