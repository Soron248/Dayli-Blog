using Dayli_Blog.Data;
using Dayli_Blog.Models.DTOs;
using Dayli_Blog.Models.Entities;
using Dayli_Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dayli_Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        //private readonly IWebHostEnvironment environment; // for image local
        private readonly IImageService imageService; // for image cloud


        public BlogController(ApplicationDbContext dbContext, IImageService imageService)
        {
            this.dbContext = dbContext;
            this.imageService = imageService;
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
                imagePath = await imageService.UploadImageAsync(dto.Image);
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromForm] UpdateBlogDto dto)
        {
            // ── Step 1: Blog খুঁজে বের করো ────────────────────
            var blog = await dbContext.Blogs.FindAsync(id);

            if (blog is null)
                return NotFound("Blog is not exist!");

            // ── Step 2: Text fields update করো ─────────────────
            if (!string.IsNullOrEmpty(dto.Title))
                blog.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                blog.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Location))
                blog.Location = dto.Location;

            // ── Step 3: Image update করো ───────────────────────
            if (dto.Image is not null && dto.Image.Length > 0)
            {
                // পুরনো image Cloudinary থেকে delete
                if (!string.IsNullOrEmpty(blog.ImagePath))
                    await imageService.DeleteImageAsync(blog.ImagePath);

                // নতুন image upload
                blog.ImagePath = await imageService.UploadImageAsync(dto.Image);
            }

            // ── Step 4: DB Save ─────────────────────────────────
            await dbContext.SaveChangesAsync();
            // UpdatedAt → DbContext এ automatically set হবে ✅

            // ── Step 5: Response ────────────────────────────────
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

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            // ── Step 1: Blog খুঁজে বের করো ────────────────────
            var blog = await dbContext.Blogs.FindAsync(id);

            if (blog is null)
                return NotFound("Blog পাওয়া যায়নি!");

            // ── Step 2: Image delete করো ───────────────────────
            if (!string.IsNullOrEmpty(blog.ImagePath))
            {
                await imageService.DeleteImageAsync(blog.ImagePath);
            }

            // ── Step 3: DB থেকে delete করো ────────────────────
            dbContext.Blogs.Remove(blog);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
