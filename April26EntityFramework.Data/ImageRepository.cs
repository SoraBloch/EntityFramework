using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace April26EntityFramework.Data
{
    public class ImageRepository
    {
        private string _connectionString;
        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;   
        }
        public List<Image> GetAllImages()
        {
            using var context = new ImageDbContext(_connectionString);
            return context.Images.OrderByDescending(d => d.DateCreated).ToList();
        }
        public void AddImage(Image image)
        {
            using var context = new ImageDbContext(_connectionString);
            context.Images.Add(image);
            context.SaveChanges();
        }
        public Image GetImageById(int id)
        {
            using var context = new ImageDbContext(_connectionString);
            return context.Images.FirstOrDefault(d => d.Id == id);
        }

        public void LikeImage(int id)
        {
            using var context = new ImageDbContext(_connectionString);
            var image = GetImageById(id);
            image.Likes++;
            context.Images.Attach(image);
            context.Entry(image).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
