using MediatR;

namespace ByShutterBackend.Application.FileStorage.Commands;

public record UploadImageCommand(
    Stream Stream,
    string DisplayName,
    string Directory
) : IRequest;