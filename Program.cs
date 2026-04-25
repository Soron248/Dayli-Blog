using Dayli_Blog.Data;
using Dayli_Blog.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB context add
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IImageService, ImageService>();

// CORS add
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins("https://blog-eight-lac-37.vercel.app", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
// ✅ এটা add করো
try
{
    app.Run($"http://0.0.0.0:{port}");
}
catch (Exception ex)
{
    Console.WriteLine("=== STARTUP ERROR ===");
    Console.WriteLine(ex.GetType().FullName);
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
    throw;
}
