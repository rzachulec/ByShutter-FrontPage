using ByShutterBackend.Domain.Common.Interfaces;
using MediatR;

namespace ByShutterBackend.Application.FileStorage.Commands;

public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand>
{
    private readonly IFileStorage _fileStorage;

    public UploadImageCommandHandler(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        await _fileStorage.UploadFileAsync(request.DisplayName, request.Directory, request.Stream);
    }
}