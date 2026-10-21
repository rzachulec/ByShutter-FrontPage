namespace ByShutterBackend.Web.Requests;

public record CreateAlbumRequest(
    string Name,
    string Description
    );