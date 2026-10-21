using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands.Albums;

public class AddImagesToAlbumCommandHandler : IRequestHandler<AddImagesToAlbumCommand>
{
    private readonly IAlbumRepository _albumRepository;

    public AddImagesToAlbumCommandHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task Handle(AddImagesToAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await _albumRepository.GetAlbumAsync(request.AlbumId, cancellationToken);

        if (album is null)
        {
            throw new NotFoundException(nameof(Album), request.AlbumId);
        }

        foreach (var imageId in request.ImageIds)
        {
            var albumPhoto = new AlbumPhoto(album.Id, imageId);
            
            album.AddPhoto(albumPhoto);
        }
        
        await _albumRepository.UpdateAlbumAsync(album, cancellationToken);
    }
}