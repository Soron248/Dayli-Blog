using Dayli_Blog.Data;
using Dayli_Blog.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dayli_Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext; 
        private readonly IWebHostEnvironment environment; // for image


        public BlogController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            this.dbContext = dbContext;
            this.environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await dbContext.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BlogResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Location = b.Location,
                    ImagePath = b.ImagePath,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToListAsync();

            return Ok(blogs);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBlogById(Guid id)
        {
            var blog = await dbContext.Blogs.FindAsync(id);

            if (blog is null)
                return NotFound("Blog is not found!");

            var response = new BlogResponseDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Location = blog.Location,
                ImagePath = blog.ImagePath,
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt
            };

            return Ok(response);
        }
    }
}
