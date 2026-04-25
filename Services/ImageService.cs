using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Dayli_Blog.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        // ── Image Upload ────────────────────────────────
        public async Task<string> UploadImageAsync(IFormFile image)
        {
            await using var stream = image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, stream),
                Folder = "dayli-blog" // Cloudinary তে folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString(); // https://res.cloudinary.com/...
        }

        // ── Image Delete ────────────────────────────────
        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            // URL থেকে PublicId বের করো
            var uri = new Uri(imageUrl);
            var segments = uri.Segments;
            var publicId = string.Join("", segments)
                .Split("dayli-blog/")[1]
                .Replace(".jpg", "")
                .Replace(".png", "")
                .Replace(".jpeg", "")
                .Trim('/');

            var deleteParams = new DeletionParams($"dayli-blog/{publicId}");
            await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}