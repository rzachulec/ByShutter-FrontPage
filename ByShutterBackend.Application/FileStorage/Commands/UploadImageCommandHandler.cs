using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
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
        await _fileStorage.UploadFileAsync(
            Path.Combine(Photo.OriginalsDirectory, request.OriginalName),
            request.Directory,
            request.Original);

        await using var web = new MemoryStream(request.WebImage, writable: false);
        await _fileStorage.UploadFileAsync(request.WebName, request.Directory, web);
    }
}
