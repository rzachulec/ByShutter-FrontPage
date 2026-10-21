using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Photos;

public sealed record GetPhotosNotInAlbumPagedQuery(
    Guid AlbumId,
    int Skip,
    int Take
) : IRequest<Result<IEnumerable<PhotoDto>>>;

public sealed class GetPhotosNotInAlbumPagedQueryHandler : IRequestHandler<GetPhotosNotInAlbumPagedQuery, Result<IEnumerable<PhotoDto>>>
{
    private readonly IPhotoRepository _photoRepository;

    public GetPhotosNotInAlbumPagedQueryHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task<Result<IEnumerable<PhotoDto>>> Handle(GetPhotosNotInAlbumPagedQuery request, CancellationToken cancellationToken)
    {
        var photos = await _photoRepository.GetPhotosNotInAlbumAsync(
            request.AlbumId,
            request.Skip,
            request.Take,
            cancellationToken
        );

        return Result.Ok(photos.Select(p => new PhotoDto(
            p.Id,
            p.FileName,
            p.ServerPath,
            p.ImageAlt,
            p.UploadedAt,
            p.DisplayName,
            p.Description
        )));
    }
}