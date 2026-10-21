using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands;

public class CreatePhotoMetaCommandHandler : IRequestHandler<CreatePhotoMetaCommand>
{
    private readonly IPhotoRepository _photoRepository;

    public CreatePhotoMetaCommandHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task Handle(CreatePhotoMetaCommand request, CancellationToken cancellationToken)
    {
        await _photoRepository.CreatePhotoAsync(new Photo(request.FileName, request.ServerPath, request.ImageAlt)
        {
            DisplayName = request.DisplayName ?? string.Empty,
            Description = request.Description ?? string.Empty,
        }, cancellationToken);
    }
}