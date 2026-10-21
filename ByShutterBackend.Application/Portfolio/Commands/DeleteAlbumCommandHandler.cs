using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands;

public class DeleteAlbumCommandHandler : IRequestHandler<DeleteAlbumCommand, Result>
{
    private readonly IAlbumRepository _albumRepository;

    public DeleteAlbumCommandHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<Result> Handle(DeleteAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await _albumRepository.GetAlbumAsync(request.AlbumId, cancellationToken);

        if (album is null)
        {
            return Result.Fail($"Album with key {request.AlbumId} not found.");
        }

        await _albumRepository.DeleteAlbumAsync(album, cancellationToken);

        return Result.Ok();
    }
}
