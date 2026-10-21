using ByShutterBackend.Application.Portfolio.DTO;
using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Queries.Photos;

public class GetPhotosBatchQueryHandler : IRequestHandler<GetPhotosBatchQuery, IEnumerable<PhotoDto>>
{
    private readonly IPhotoRepository _photoRepository;

    public GetPhotosBatchQueryHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task<IEnumerable<PhotoDto>> Handle(GetPhotosBatchQuery request, CancellationToken cancellationToken)
    {
        var skip = request.Page * request.PageSize;
        var take = request.PageSize;

        var batch = await _photoRepository.GetPhotosAsync((uint)skip, take, cancellationToken);

        var batchDto = batch.Select(photo => new PhotoDto(
            photo.Id,
            photo.FileName,
            photo.ServerPath,
            photo.ImageAlt,
            photo.UploadedAt,
            photo.DisplayName,
            photo.Description
        ));
        
        return batchDto;
    }
}