using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands.Albums;

public class RemoveImagesFromAlbumCommandHandler : IRequestHandler<RemoveImagesFromAlbumCommand, Result>
{
    private readonly IAlbumRepository _albumRepository;

    public RemoveImagesFromAlbumCommandHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<Result> Handle(RemoveImagesFromAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await _albumRepository.GetAlbumAsync(request.AlbumId, cancellationToken);

        if (album is null)
        {
            return Result.Fail($"Album with key {request.AlbumId} not found.");
        }

        album.RemovePhotos(request.ImageIds);

        await _albumRepository.UpdateAlbumAsync(album, cancellationToken);

        return Result.Ok();
    }
}