using ByShutterBackend.Application.Common.Exceptions;
using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Photos;

public class GetPhotoByFileNameQueryHandler : IRequestHandler<GetPhotoByFileNameQuery, PhotoDto>
{
    private readonly IPhotoRepository _photoRepository;

    public GetPhotoByFileNameQueryHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task<PhotoDto> Handle(GetPhotoByFileNameQuery request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetPhotoByFileNameAsync(request.FileName, cancellationToken);

        if (photo is null)
        {
            throw new NotFoundException(nameof(Photo), request.FileName);
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