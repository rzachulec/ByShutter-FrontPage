using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Photos;

public class GetPhotoQueryHandler : IRequestHandler<GetPhotoQuery, PhotoDto>
{
    private readonly IPhotoRepository _photoRepository;

    public GetPhotoQueryHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task<PhotoDto> Handle(GetPhotoQuery request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetPhotoAsync(request.PhotoId, cancellationToken);

        if (photo is null)
        {
            throw new NotFoundException(nameof(Photo), request.PhotoId);
        }

        return new PhotoDto(
            photo.Id,
            photo.FileName,
            photo.ServerPath,
            photo.ImageAlt,
            photo.UploadedAt,
            photo.DisplayName,
            photo.Description
        );
    }
}