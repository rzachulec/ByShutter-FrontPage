using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Albums;

public class GetAlbumByNameQueryHandler : IRequestHandler<GetAlbumByNameQuery, AlbumDto>
{
    private readonly IAlbumRepository _albumRepository;

    public GetAlbumByNameQueryHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<AlbumDto> Handle(GetAlbumByNameQuery request, CancellationToken cancellationToken)
    {
        var album = await _albumRepository.GetAlbumByNameAsync(request.AlbumName, cancellationToken);

        if (album is null)
        {
            throw new NotFoundException(nameof(album), request.AlbumName);
        }

        var albumPhotoDto = album.Photos.Select(p => new AlbumPhotoDto(
            p.AlbumId,
            p.PhotoId,
            p.AddedToAlbumAt
        ));
        
        return new AlbumDto(
            album.Id,
            album.Name,
            album.Description,
            albumPhotoDto
        );
    }
}