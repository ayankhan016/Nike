using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using P2WebMVC.Interfaces;
using P2WebMVC.Settings;

namespace P2WebMVC.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        if (string.IsNullOrEmpty(config.Value.CloudinaryUrl))
            throw new ArgumentNullException("Cloudinary URL is not configured.");

        _cloudinary = new Cloudinary(config.Value.CloudinaryUrl)
        {
            Api = { Secure = true }
        };
    }

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
            throw new ArgumentException("Image is missing.");

        using var stream = image.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream),
            UniqueFilename = false,
            Overwrite = true,
            Folder = "Trinkle"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return result.SecureUrl.ToString();
    }

    public async Task<string> UploadVideoAsync(IFormFile video)
    {
        if (video == null || video.Length == 0)
            throw new ArgumentException("Video is missing.");

        using var stream = video.OpenReadStream();

        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(video.FileName, stream),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true,
            Folder = "Trinkle"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return result.SecureUrl.ToString();
    }

    public Task<string> UploadMultipleImageAsync(IFormFile imgArr)
    {
        throw new NotImplementedException();
    }
}
