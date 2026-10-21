namespace ByShutterBackend.Application.Portfolio.DTO;

public record PhotoDto(
    Guid Id,
    string FileName,
    string ServerPath,
    string ImageAlt,
    DateTime UploadedAt,
    string? DisplayName,
    string? Description
);