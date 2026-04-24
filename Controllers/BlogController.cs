using Dayli_Blog.Data;
using Dayli_Blog.Models.DTOs;
using Dayli_Blog.Models.Entities;
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

        [HttpPost]
        public async Task<IActionResult> CreateBlog([FromForm] CreateBlogDto dto)
        {
            string? imagePath = null;

            // ── Step 1: Image handling ──────────────────────────
            if (dto.Image is not null && dto.Image.Length > 0)
            {
                // Unique file name তৈরি করো
                var fileName = Guid.NewGuid().ToString()
                               + Path.GetExtension(dto.Image.FileName);
                // "a1b2c3d4-xxxx.jpg"

                // wwwroot/images folder এর full path
                var folderPath = Path.Combine(environment.WebRootPath, "images");

                // Folder না থাকলে তৈরি করো
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Full file path
                var fullFilePath = Path.Combine(folderPath, fileName);
                // "C:/project/wwwroot/images/a1b2c3d4-xxxx.jpg"

                // File টা wwwroot/images এ save করো
                await using var stream = new FileStream(fullFilePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                // DB তে শুধু relative path রাখো
                imagePath = $"images/{fileName}";
                // "images/a1b2c3d4-xxxx.jpg"
            }

            // ── Step 2: Blog object তৈরি ───────────────────────
            var blog = new Blog
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                ImagePath = imagePath  // null হতে পারে, সেটা okay
            };

            // ── Step 3: DB তে save করো ─────────────────────────
            await dbContext.Blogs.AddAsync(blog);
            await dbContext.SaveChangesAsync();

            // ── Step 4: Response তৈরি করো ──────────────────────
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

            return CreatedAtAction(nameof(GetBlogById), new { id = blog.Id }, response);
        }
    }
}
