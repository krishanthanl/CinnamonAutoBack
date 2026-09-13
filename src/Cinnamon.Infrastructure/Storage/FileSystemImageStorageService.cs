using Cinnamon.Application.Interfaces;

namespace Cinnamon.Infrastructure.Storage;

/// <summary>Saves base64-encoded images under a configured directory and returns an uploaded-image URL path.</summary>
public class FileSystemImageStorageService : IImageStorageService
{
    private readonly string _imagesFolder;
    private const string UrlPrefix = "/uploads/parts";

    public FileSystemImageStorageService(string imagesFolder)
    {
        _imagesFolder = imagesFolder;
    }

    public async Task<string> SaveAsync(string base64Data, string fileNameWithoutExtension, string? extension)
    {
        if (!Directory.Exists(_imagesFolder))
        {
            Directory.CreateDirectory(_imagesFolder);
        }

        var ext = string.IsNullOrWhiteSpace(extension) ? "jpg" : extension.TrimStart('.');
        var filename = $"{fileNameWithoutExtension}.{ext}";
        var filepath = Path.Combine(_imagesFolder, filename);

        var data = base64Data;
        var commaIndex = data.IndexOf(',');
        if (commaIndex >= 0)
        {
            data = data[(commaIndex + 1)..];
        }

        var bytes = Convert.FromBase64String(data);
        await File.WriteAllBytesAsync(filepath, bytes);

        return $"{UrlPrefix}/{filename}";
    }
}
