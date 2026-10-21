namespace ByShutterBackend.Web.Requests;

public record CreatePortfolioTileRequest(
    string Name,
    Guid ImageId,
    string? Description
);