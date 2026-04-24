namespace Dayli_Blog.Models.Entities
{
    public class Blog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Title { get; set; }      
        public required string Description { get; set; } 
        public required string Location { get; set; }  
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
    }
}
