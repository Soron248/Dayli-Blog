namespace Dayli_Blog.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile image);
        Task DeleteImageAsync(string imageUrl);
    }
}
