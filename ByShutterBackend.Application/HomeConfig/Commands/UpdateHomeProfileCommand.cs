using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public record UpdateHomeProfileCommand(
    Guid HomeProfileId,
    string HeroMessage,
    string Greetings,
    string Quote,
    string AboutMe,
    Guid HeroImageId,
    Guid ProfileImageId
) : IRequest<Result>;