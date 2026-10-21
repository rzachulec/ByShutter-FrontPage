using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Albums;

public class GetAlbumByIdQueryHandler : IRequestHandler<GetAlbumByIdQuery, AlbumDto>
{
    private readonly IAlbumRepository _albumRepository;

    public GetAlbumByIdQueryHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<AlbumDto> Handle(GetAlbumByIdQuery request, CancellationToken cancellationToken)
    {
        var album = await _albumRepository.GetAlbumAsync(request.AlbumId, cancellationToken);

        if (album is null)
        {
            throw new NotFoundException(nameof(Album), request.AlbumId);
        }

        var photosDto = album.Photos.Select(p => new AlbumPhotoDto(
            p.AlbumId,
            p.PhotoId,
            p.AddedToAlbumAt
        ));

        return new AlbumDto(
            album.Id,
            album.Name,
            album.Description,
            photosDto
        );
    }
}