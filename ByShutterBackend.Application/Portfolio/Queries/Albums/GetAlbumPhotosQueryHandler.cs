using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using FluentResults;
using MediatR;


namespace ByShutterBackend.Application.Portfolio.Queries.Albums;

public class GetAlbumPhotosQueryHandler : IRequestHandler<GetAlbumPhotosQuery, Result<IEnumerable<PhotoDto>>>
{
    private readonly IAlbumRepository _albumRepository;

    public GetAlbumPhotosQueryHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<Result<IEnumerable<PhotoDto>>> Handle(GetAlbumPhotosQuery request, CancellationToken cancellationToken)
    {
        var response = await _albumRepository.GetAlbumPhotosAsync(
            request.AlbumId,
            request.Skip,
            request.Take,
            cancellationToken
        );

        return Result.Ok(response.Select(ap => new PhotoDto(
            ap.Photo.Id,
            ap.Photo.FileName,
            ap.Photo.ServerPath,
            ap.Photo.ImageAlt,
            ap.Photo.UploadedAt,
            ap.Photo.DisplayName,
            ap.Photo.Description
        )));
    }
}