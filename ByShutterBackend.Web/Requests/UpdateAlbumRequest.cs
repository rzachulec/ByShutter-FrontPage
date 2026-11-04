namespace ByShutterBackend.Web.Requests;

public record UpdateAlbumRequest(
    string Name,
    string Description,
    DateTime? Date
);
