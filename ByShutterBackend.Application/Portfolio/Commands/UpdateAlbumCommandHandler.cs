using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands;

public class UpdateAlbumCommandHandler : IRequestHandler<UpdateAlbumCommand, Result>
{
    public const string StatusKey = "status";

    private readonly IAlbumRepository _albumRepository;

    public UpdateAlbumCommandHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<Result> Handle(UpdateAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await _albumRepository.GetAlbumAsync(request.AlbumId, cancellationToken);
        if (album is null)
        {
            return Result.Fail(new Error($"Album {request.AlbumId} not found.").WithMetadata(StatusKey, 404));
        }

        // Album names are unique; report a clash instead of failing on the database index.
        var sameName = await _albumRepository.GetAlbumByNameAsync(request.Name, cancellationToken);
        if (sameName is not null && sameName.Id != album.Id)
        {
            return Result.Fail(new Error($"An album named \"{request.Name}\" already exists.").WithMetadata(StatusKey, 409));
        }

        album.Update(request.Name, request.Description, request.Date);
        await _albumRepository.UpdateAlbumAsync(album, cancellationToken);

        return Result.Ok();
    }
}
