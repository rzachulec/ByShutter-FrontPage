using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands.Photos;

public record DeleteImageCommand(Guid ImageId) : IRequest;