namespace ByShutterBackend.Application.Portfolio.DTO;

public record AlbumImagesDto(
    IEnumerable<PhotoDto> Photos
);