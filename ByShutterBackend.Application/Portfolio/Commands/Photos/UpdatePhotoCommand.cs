using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands.Photos;

public record UpdatePhotoCommand(
    Guid PhotoId,
    string? DisplayName = null,
    string? Description = null,
    string? PhotoAlt = null
) : IRequest;