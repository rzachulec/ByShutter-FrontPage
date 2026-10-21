namespace ByShutterBackend.Application.Portfolio.DTO;

public record AlbumHeaderDto(
    Guid albumId,
    string Name,
    string Description);