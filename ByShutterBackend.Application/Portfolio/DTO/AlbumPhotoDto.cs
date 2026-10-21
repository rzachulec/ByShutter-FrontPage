namespace ByShutterBackend.Application.Portfolio.DTO;

public record AlbumPhotoDto(
    Guid AlbumId,
    Guid PhotoId,
    DateTime UploadedAt
);