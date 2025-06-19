using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookShoppingCartMVC.Shared;


namespace BookShoppingCartMvcUI.Shared
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFile(IFormFile file, string[] allowedExtensions)
        {
            var wwwPath = _environment.WebRootPath;
            var path = Path.Combine(wwwPath, "images");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Only {string.Join(", ", allowedExtensions)} files allowed");
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fileNameWithPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        public void DeleteFile(string fileName)
        {
            var wwwPath = _environment.WebRootPath;
            var fileNameWithPath = Path.Combine(wwwPath, "images", fileName);

            if (!File.Exists(fileNameWithPath))
            {
                throw new FileNotFoundException(fileName);
            }

            File.Delete(fileNameWithPath);
        }
    }
}
