using FluentResults;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands;

public record UpdateAlbumCommand(Guid AlbumId, string Name, string Description, DateTime? Date) : IRequest<Result>;
