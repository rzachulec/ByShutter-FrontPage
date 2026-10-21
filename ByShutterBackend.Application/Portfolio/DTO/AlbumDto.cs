namespace ByShutterBackend.Application.Portfolio.DTO;

public record AlbumDto(
    Guid Id,
    string Name,
    string Description,
    IEnumerable<AlbumPhotoDto> Photos
);