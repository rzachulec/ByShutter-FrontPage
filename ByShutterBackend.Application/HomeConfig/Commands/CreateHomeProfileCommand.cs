using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public record CreateHomeProfileCommand(
    string HeroMessage,
    string Greetings,
    string Quote,
    string AboutMe,
    Guid HeroImageId,
    Guid ProfileImageId
) : IRequest<Result<Guid>>;