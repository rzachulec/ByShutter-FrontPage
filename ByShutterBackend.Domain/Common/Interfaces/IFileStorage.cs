namespace ByShutterBackend.Domain.Common.Interfaces;

public interface IFileStorage
{
    public Task UploadFileAsync(string displayName, string directory, Stream stream);
    public Task DeleteFileAsync(string fileName, string path);
}