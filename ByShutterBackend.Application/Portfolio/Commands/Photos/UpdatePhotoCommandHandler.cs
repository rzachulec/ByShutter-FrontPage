using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands.Photos;

public class UpdatePhotoCommandHandler : IRequestHandler<UpdatePhotoCommand>
{
    private readonly IPhotoRepository _photoRepository;

    public UpdatePhotoCommandHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task Handle(UpdatePhotoCommand request, CancellationToken cancellationToken)
    {
        var photo = await _photoRepository.GetPhotoAsync(request.PhotoId, cancellationToken);

        if (photo is null)
        {
            // TODO: result pattern
            return;
        }
        
        photo.DisplayName = request.DisplayName ?? photo.DisplayName;
        photo.Description = request.Description ?? photo.Description;
        photo.ImageAlt = request.PhotoAlt ?? photo.ImageAlt;
        
        await _photoRepository.UpdatePhotoAsync(photo, cancellationToken);
    }
}