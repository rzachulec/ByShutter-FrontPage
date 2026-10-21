namespace ByShutterBackend.Web.Requests;

public record UploadPhotoRequest(
    IFormFile Photo,
    string FileName,
    string ImageAlt,
    string? DisplayName = null,
    string? Description = null
);