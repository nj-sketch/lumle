using Lumle.Infrastructure.Utilities.Abstracts;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using NLog;
using Lumle.Infrastructure.Constants.LumleLog;
using SixLabors.ImageSharp;
using SixLabors.Primitives;

namespace Lumle.Infrastructure.Utilities
{
    public class FileHandler : IFileHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IHostingEnvironment _environment;

        public FileHandler(IHostingEnvironment environment)
        {
            _environment = environment;
        }


        public string UploadImage(IFormFile sourceImage, int width, int height)
        {
            try
            {
                int resizeWidth, resizeHeight, sourceX, sourceY, size;

                var directory = Path.Combine(_environment.WebRootPath, "uploadedimages");

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var stream = sourceImage.OpenReadStream();
                var image = Image.Load(stream);

                // For image where resize width and height is not provided
                if(height == 0 && width == 0)
                {
                    width = image.Width;
                    height = image.Height;
                }

                var imageName = $"{ Guid.NewGuid()}_{width}X{height}_{sourceImage.FileName}";
                var imagePath = Path.Combine(directory, imageName);

                if (image.Width > image.Height)
                {
                    size = height;
                    resizeWidth = Convert.ToInt32(image.Width * size / (double)image.Height);
                    resizeHeight = size;
                }
                else
                {
                    size = width;
                    resizeWidth = size;
                    resizeHeight = Convert.ToInt32(image.Height * size / (double)image.Width);

                }

                sourceX = Convert.ToInt32((resizeWidth / (double)2) - (width / (double)2));
                sourceY = Convert.ToInt32((resizeHeight / (double)2) - (height / (double)2));

                var destinationImage = CropImage(image, sourceX, sourceY, width, height, resizeWidth, resizeHeight);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    destinationImage.SaveAsPng(fileStream);

                }
                return imageName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.ImageUploadError);
                throw new Exception(ex.Message);
            }
        }

        private Image<Rgba32> CropImage(Image<Rgba32> sourceImage, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
        {
            sourceImage.Mutate(x => x.Resize(destinationWidth, destinationHeight)
                        .Crop(new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight)));

            return sourceImage;
        }

    }
}
