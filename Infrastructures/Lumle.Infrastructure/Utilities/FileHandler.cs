using Lumle.Infrastructure.Utilities.Abstracts;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using ImageSharp;
using ImageSharp.PixelFormats;
using Lumle.Infrastructure.Constants.Log;
using Microsoft.AspNetCore.Hosting;
using NLog;

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

                var imageName = $"{ Guid.NewGuid()}_{width}X{height}_{sourceImage.FileName}";
                var imagePath = Path.Combine(directory, imageName);


                var stream = sourceImage.OpenReadStream();
                var image = Image.Load(stream);

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
                    destinationImage.Save(fileStream);

                }
                return imageName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.ImageUploadError);
                throw new Exception(ex.Message);
            }
        }
        //private string UploadOriginalImage(IFormFile sourceImage)
        //{
        //    try
        //    {
        //        var directory = Path.Combine(_environment.WebRootPath, "uploadedimages");
        //        var imagePath = Path.Combine(directory, $"{Guid.NewGuid()}_original_{sourceImage.FileName}");

        //        Image image = null;
        //        var stream = sourceImage.OpenReadStream();
        //        image = Image.Load(stream);

        //        var destinationImage = CropImage(image, 0, 0, image.Width, image.Height, image.Width, image.Height);

        //        using (var fileStream = new FileStream(imagePath, FileMode.Create))
        //        {
        //            destinationImage.Save(fileStream);

        //        }
        //        return imagePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        private Image<Rgba32> CropImage(Image sourceImage, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
        {
            return sourceImage
                 .Resize(destinationWidth, destinationHeight)
                 .Crop(new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight));
        }

    }
}
