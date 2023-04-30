using April26EntityFramework.Data;
using April26EntityFramework.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace April26EntityFramework.Web.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _environment;
        private string _connectionString;
        public HomeController(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index()
        {
            var repo = new ImageRepository(_connectionString);

            return View(new IndexViewModel
            {
                Images = repo.GetAllImages()
            });
        }
        public IActionResult UploadImage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UploadImage(Image image, IFormFile imageFile)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            string fullPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            using var stream = new FileStream(fullPath, FileMode.CreateNew);
            imageFile.CopyTo(stream);
            image.FileName = fileName;
            image.DateCreated = DateTime.Now;
            image.Likes = 0;

            var repo = new ImageRepository(_connectionString);
            repo.AddImage(image);
            return RedirectToAction("Index");
        }
        public IActionResult ViewImage(int id)
        {
            var sessonIds = HttpContext.Session.Get<List<int>>("ids");
            bool disableLike = false;
            if (sessonIds != null && sessonIds.Contains(id))
            {
                disableLike = true;
            }
            var repo = new ImageRepository(_connectionString);
            return View(new ViewImageViewModel
            {
                Image = repo.GetImageById(id),
                DisableLike = disableLike
            });
        }
        [HttpPost]
        public void LikeImage(int id)
        {
            var sessionIds = HttpContext.Session.Get<List<int>>("ids");
           
            if (sessionIds == null)
            {
                sessionIds = new List<int>();
            }
            if (!sessionIds.Contains(id))
            {
                var repo = new ImageRepository(_connectionString);
                repo.LikeImage(id);
            }
            sessionIds.Add(id);
            HttpContext.Session.Set("ids", sessionIds);
        }
        public IActionResult GetLikesForImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            var image = repo.GetImageById(id);
            return Json(image.Likes);
        }

    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}