namespace ByShutterBackend.Application.Portfolio.DTO;

public record AlbumMetaDataDto(
    Guid Id,
    string Name,
    string Description,
    int ItemsCount
);