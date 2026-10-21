using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Albums;

public class GetAlbumsQueryHandler : IRequestHandler<GetAlbumsQuery, IEnumerable<AlbumDto>>
{
    private readonly IAlbumRepository _albumRepository;

    public GetAlbumsQueryHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<IEnumerable<AlbumDto>> Handle(GetAlbumsQuery request, CancellationToken cancellationToken)
    {
        var take = request.PageSize;
        var skip = request.Page * request.PageSize;

        var albums = await _albumRepository
            .GetAlbumsAsync(skip, take, cancellationToken);

        var albumDto = albums.Select(a => new AlbumDto(
            a.Id,
            a.Name,
            a.Description,
            a.Photos.Select(p => new AlbumPhotoDto(
                p.AlbumId,
                p.PhotoId,
                p.AddedToAlbumAt
            ))
        ));
        
        return albumDto;
    }
}