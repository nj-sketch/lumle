using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Lumle.Infrastructure.Utilities.Abstracts
{
    public interface IFileHandler
    {
        string UploadImage(IFormFile sourceImage, int width, int height);
        Task<string> SaveFile(byte[] bytes, string fileName, string storePath, string suffix = null);
        Task<string> SaveImageFromEditor(string content, string storePath);
    }
}
