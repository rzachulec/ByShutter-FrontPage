using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands.Photos;

public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
{
    private readonly IPhotoRepository _photoRepository;

    public DeleteImageCommandHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        var image = await _photoRepository.GetPhotoAsync(request.ImageId, cancellationToken);

        if (image is null)
        {
            return;
        }
    
        await _photoRepository.DeletePhotoAsync(image, cancellationToken);
    }
}