using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.HomeConfig;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public class CreateHomeProfileCommandHandler : IRequestHandler<CreateHomeProfileCommand, Result<Guid>>
{
    private readonly IHomeProfileRepository _homeProfileRepository;

    public CreateHomeProfileCommandHandler(IHomeProfileRepository homeProfileRepository)
    {
        _homeProfileRepository = homeProfileRepository;
    }

    public async Task<Result<Guid>> Handle(CreateHomeProfileCommand request, CancellationToken cancellationToken)
    {
        var homeProfile = new HomeProfile(
            request.HeroMessage,
            request.Greetings,
            request.Quote,
            request.AboutMe,
            request.HeroImageId,
            request.ProfileImageId
        );

        await _homeProfileRepository.CreateHomeProfileAsync(homeProfile, cancellationToken);

        return Result.Ok(homeProfile.Id);
    }
}