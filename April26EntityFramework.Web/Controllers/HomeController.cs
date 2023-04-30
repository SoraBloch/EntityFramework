using April26EntityFramework.Data;
using April26EntityFramework.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

            var repo = new ImageRepository(_connectionString);
            repo.AddImage(image);
            return RedirectToAction("Index");
        }
        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            return View(new ViewImageViewModel
            {
                Image = repo.GetImageById(id),
            });
        }
        [HttpPost]
        public void LikeImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            repo.LikeImage(id);  
        }
        public IActionResult GetLikesForImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            var image = repo.GetImageById(id);
            return Json(image.Likes);
        }
    }
}