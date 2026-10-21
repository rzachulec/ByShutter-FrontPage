using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands;

public record DeleteAlbumCommand(Guid AlbumId) : IRequest<Result>;
