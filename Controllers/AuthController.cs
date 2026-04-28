using Dayli_Blog.Data;
using Dayli_Blog.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dayli_Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfiguration configuration;

        public AuthController(
            ApplicationDbContext dbContext,
            IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // ── Step 1: User খুঁজো ─────────────────────────
            var user = dbContext.Users
                .FirstOrDefault(u => u.Email == dto.Email);

            if (user is null)
                return Unauthorized("Invalid email or password!");

            // ── Step 2: Password verify করো ────────────────
            var isPasswordValid = BCrypt.Net.BCrypt
                .Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized("Invalid email or password!");

            // ── Step 3: JWT Token তৈরি করো ─────────────────
            var token = GenerateJwtToken(user.Id, user.Email);

            return Ok(new { token });
        }

        // ── Token Generate Method ───────────────────────────
        private string GenerateJwtToken(Guid userId, string email)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
            };

            var expiry = DateTime.UtcNow.AddHours(
                double.Parse(configuration["Jwt:ExpiryHours"]!));

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}