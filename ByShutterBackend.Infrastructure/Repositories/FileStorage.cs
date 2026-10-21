using ByShutterBackend.Domain.Common.Interfaces;

namespace ByShutterBackend.Infrastructure.Repositories;

public class FileStorage : IFileStorage
{
    public async Task UploadFileAsync(string displayName, string directory, Stream stream)
    {
        Console.WriteLine("directory: " + directory);
        var path = Path.Combine(directory, displayName);

        await using var fs = File.Create(path);
        await stream.CopyToAsync(fs);
    }

    public async Task DeleteFileAsync(string fileName, string path)
    {
        throw new NotImplementedException();
    }
}