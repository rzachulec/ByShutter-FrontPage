using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands;

public record CreatePhotoMetaCommand(
    string FileName,
    string ServerPath,
    string ImageAlt,
    string? DisplayName = null,
    string? Description = null
) : IRequest;