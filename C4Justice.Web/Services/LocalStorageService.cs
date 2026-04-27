namespace C4Justice.Web.Services;

public interface IStorageService
{
    Task<string?> UploadImageAsync(IFormFile file, string folder);
    Task<string?> UploadRawAsync(IFormFile file, string folder);
    Task DeleteAsync(string url, bool isRaw = false);
}

public class LocalStorageService : IStorageService
{
    private readonly string _wwwroot;
    private static readonly HashSet<string> _imageExts =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public LocalStorageService(IWebHostEnvironment env)
    {
        _wwwroot = env.WebRootPath;
    }

    public async Task<string?> UploadImageAsync(IFormFile file, string folder)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_imageExts.Contains(ext)) return null;
        return await SaveFile(file, folder, ext);
    }

    public async Task<string?> UploadRawAsync(IFormFile file, string folder)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        return await SaveFile(file, folder, ext);
    }

    public Task DeleteAsync(string url, bool isRaw = false)
    {
        if (string.IsNullOrWhiteSpace(url)) return Task.CompletedTask;
        // url is like /uploads/articles/abc123.jpg — map to physical path
        var relative = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_wwwroot, relative);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }

    private async Task<string?> SaveFile(IFormFile file, string folder, string ext)
    {
        // "c4justice/articles" → "articles", "c4justice/slider" → "slider", etc.
        var subfolder = folder.Contains('/')
            ? folder[(folder.LastIndexOf('/') + 1)..]
            : folder;

        var dir = Path.Combine(_wwwroot, "uploads", subfolder);
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(dir, fileName);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        return $"/uploads/{subfolder}/{fileName}";
    }
}
