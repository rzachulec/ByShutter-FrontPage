using ByShutterBackend.Domain.Common.Interfaces;
using ByShutterBackend.Domain.Portfolio;
using MediatR;

namespace ByShutterBackend.Application.Portfolio.Commands;

public class CreateAlbumCommandHandler : IRequestHandler<CreateAlbumCommand>
{
    private readonly IAlbumRepository _albumRepository;

    public CreateAlbumCommandHandler(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task Handle(CreateAlbumCommand request, CancellationToken cancellationToken = default)
    {
        var album = new Album(name: request.AlbumName, description: request.Description);
            
        await _albumRepository.CreateAlbumAsync(album, cancellationToken);
    }
}