namespace Dayli_Blog.Models.DTOs
{
    public class UpdateBlogDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
