using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public class UpdateHomeProfileCommandHandler : IRequestHandler<UpdateHomeProfileCommand, Result>
{
    private readonly IHomeProfileRepository _homeProfileRepository;

    public UpdateHomeProfileCommandHandler(IHomeProfileRepository homeProfileRepository)
    {
        _homeProfileRepository = homeProfileRepository;
    }

    public async Task<Result> Handle(UpdateHomeProfileCommand request, CancellationToken cancellationToken)
    {
        var homeProfile = await _homeProfileRepository.GetHomeProfileAsync(request.HomeProfileId, cancellationToken);

        if (homeProfile is null)
        {
            return Result.Fail($"HomeProfile with key {request.HomeProfileId} not found.");
        }

        homeProfile.HeroMessage = request.HeroMessage;
        homeProfile.Greetings = request.Greetings;
        homeProfile.Quote = request.Quote;
        homeProfile.AboutMe = request.AboutMe;
        homeProfile.HeroImageId = request.HeroImageId;
        homeProfile.ProfileImageId = request.ProfileImageId;

        await _homeProfileRepository.UpdateHomeProfileAsync(homeProfile, cancellationToken);

        return Result.Ok();
    }
}