namespace ByShutterBackend.Web.Requests;

public record UpdatePortfolioTileRequest(
    string Name,
    Guid ImageId,
    string? Description
);