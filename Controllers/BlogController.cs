using Dayli_Blog.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
