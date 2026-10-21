using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.HomeConfig.Commands;

public record DeleteHomeProfileCommand(
    Guid HomeProfileId
) : IRequest<Result>;