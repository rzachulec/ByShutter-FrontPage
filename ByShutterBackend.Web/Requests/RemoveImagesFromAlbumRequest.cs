namespace ByShutterBackend.Web.Requests;

public record RemoveImagesFromAlbumRequest(
    IEnumerable<Guid> ImageIds
);