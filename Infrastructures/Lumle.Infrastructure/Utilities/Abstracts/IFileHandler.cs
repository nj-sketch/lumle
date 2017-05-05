using Microsoft.AspNetCore.Http;

namespace Lumle.Infrastructure.Utilities.Abstracts
{
    public interface IFileHandler
    {
        string UploadImage(IFormFile sourceImage, int width, int height);
    }
}
