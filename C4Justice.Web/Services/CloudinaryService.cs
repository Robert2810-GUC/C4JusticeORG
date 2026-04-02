using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace C4Justice.Web.Services;

public interface ICloudinaryService
{
    /// <summary>Upload an image (jpg/png/gif/webp) and return its permanent HTTPS URL.</summary>
    Task<string?> UploadImageAsync(IFormFile file, string folder);

    /// <summary>Upload a raw file (PDF, docx, etc.) and return its permanent HTTPS URL.</summary>
    Task<string?> UploadRawAsync(IFormFile file, string folder);

    /// <summary>Delete a previously-uploaded asset by its Cloudinary URL.</summary>
    Task DeleteAsync(string url, bool isRaw = false);
}

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]);
        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
    }

    public async Task<string?> UploadImageAsync(IFormFile file, string folder)
    {
        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File       = new FileDescription(file.FileName, stream),
            Folder     = folder,
            PublicId   = Guid.NewGuid().ToString("N"),
            Overwrite  = false
        };
        var result = await _cloudinary.UploadAsync(uploadParams);
        return result.Error is null ? result.SecureUrl.ToString() : null;
    }

    public async Task<string?> UploadRawAsync(IFormFile file, string folder)
    {
        await using var stream = file.OpenReadStream();
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var uploadParams = new RawUploadParams
        {
            File     = new FileDescription(file.FileName, stream),
            Folder   = folder,
            PublicId = $"{Guid.NewGuid():N}{ext}"
        };
        var result = await _cloudinary.UploadAsync(uploadParams);
        return result.Error is null ? result.SecureUrl.ToString() : null;
    }

    public async Task DeleteAsync(string url, bool isRaw = false)
    {
        var publicId = ExtractPublicId(url, isRaw);
        if (string.IsNullOrEmpty(publicId)) return;

        await _cloudinary.DestroyAsync(new DeletionParams(publicId)
        {
            ResourceType = isRaw ? ResourceType.Raw : ResourceType.Image
        });
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Parses the public_id out of a Cloudinary secure URL, e.g.:
    ///   https://res.cloudinary.com/{cloud}/image/upload/v123/{folder}/{id}.jpg
    ///   → {folder}/{id}   (extension stripped for images)
    ///   → {folder}/{id}.pdf (extension kept for raw)
    /// </summary>
    private static string ExtractPublicId(string url, bool includeExtension)
    {
        try
        {
            var path        = new Uri(url).AbsolutePath;           // /{cloud}/image/upload/v123/folder/file.ext
            var uploadToken = "/upload/";
            var idx         = path.IndexOf(uploadToken, StringComparison.Ordinal);
            if (idx < 0) return string.Empty;

            var after = path[(idx + uploadToken.Length)..];        // v123/folder/file.ext  OR  folder/file.ext

            // Strip version segment (vNNNNNNNNN/)
            if (after.Length > 1 && after[0] == 'v' && char.IsDigit(after[1]))
            {
                var slash = after.IndexOf('/');
                if (slash > 0) after = after[(slash + 1)..];      // folder/file.ext
            }

            // For images, strip the extension — Cloudinary stores images without it in the public_id
            if (!includeExtension)
            {
                var dot = after.LastIndexOf('.');
                if (dot > 0) after = after[..dot];
            }

            return after;
        }
        catch { return string.Empty; }
    }
}
