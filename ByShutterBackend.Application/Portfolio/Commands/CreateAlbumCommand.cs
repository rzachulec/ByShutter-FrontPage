using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands;

public record CreateAlbumCommand(string AlbumName, string Description) : IRequest;