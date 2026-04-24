namespace Dayli_Blog.Models.DTOs
{
    public class UpdateBlogDto
    {
        public string? Title { get; set; }         
        public string? Description { get; set; }    
        public string? Location { get; set; }      
        public IFormFile? Image { get; set; }    
    }
}