using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public class DeleteHomeProfileCommandHandler : IRequestHandler<DeleteHomeProfileCommand, Result>
{
    private readonly IHomeProfileRepository _homeProfileRepository;

    public DeleteHomeProfileCommandHandler(IHomeProfileRepository homeProfileRepository)
    {
        _homeProfileRepository = homeProfileRepository;
    }

    public async Task<Result> Handle(DeleteHomeProfileCommand request, CancellationToken cancellationToken)
    {
        var homeProfile = await _homeProfileRepository.GetHomeProfileAsync(request.HomeProfileId, cancellationToken);

        if (homeProfile is null)
        {
            return Result.Ok();
        }

        await _homeProfileRepository.DeleteHomeProfileAsync(homeProfile, cancellationToken);

        return Result.Ok();
    }
}