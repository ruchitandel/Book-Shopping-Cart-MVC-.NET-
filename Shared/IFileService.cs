using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace BookShoppingCartMVC.Shared
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile file, string[] allowedExtensions);
        void DeleteFile(string fileName);
    }
}
