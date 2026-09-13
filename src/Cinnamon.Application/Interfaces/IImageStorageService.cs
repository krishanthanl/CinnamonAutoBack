namespace Cinnamon.Application.Interfaces;

/// <summary>Persists an uploaded image and returns the URL path clients should use to fetch it.</summary>
public interface IImageStorageService
{
    Task<string> SaveAsync(string base64Data, string fileNameWithoutExtension, string? extension);
}
