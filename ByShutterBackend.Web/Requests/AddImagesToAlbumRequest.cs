namespace ByShutterBackend.Web.Requests;

public record AddImagesToAlbumRequest(
    IEnumerable<Guid> ImageIds
);