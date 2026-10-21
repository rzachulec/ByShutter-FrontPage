namespace ByShutterBackend.Web.Requests;

public record UpdatePhotoRequest(
    string? DisplayName,
    string? Description,
    string? PhotoAlt
);