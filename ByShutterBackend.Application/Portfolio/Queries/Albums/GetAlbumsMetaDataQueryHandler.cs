using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Albums;

public class GetAlbumsMetadataQueryHandler : IRequestHandler<GetAlbumsMetaDataQuery, Result<IEnumerable<AlbumMetaDataDto>>>
{
    private readonly IAlbumRepository _albumRepository;

    public GetAlbumsMetadataQueryHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<Result<IEnumerable<AlbumMetaDataDto>>> Handle(GetAlbumsMetaDataQuery request, CancellationToken cancellationToken)
    {
        var skip = request.Page * request.PageSize;
        var take = request.PageSize;
        var albums = await _albumRepository.GetAlbumsAsync(skip, take, cancellationToken);

        var albumsMetadata = albums.Select(album => new AlbumMetaDataDto(
            album.Id, 
            album.Name, 
            album.Description, 
            album.Photos.Count
        ));

        return Result.Ok(albumsMetadata);
    }
}