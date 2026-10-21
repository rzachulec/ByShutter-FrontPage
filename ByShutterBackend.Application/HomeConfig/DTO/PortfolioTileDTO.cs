namespace ByShutterBackend.Application.HomeConfig.DTO;

public record PortfolioTileDto(
    Guid Id,
    string Name,
    string? Description,
    PhotoDto Image,
    Guid ImageId
);