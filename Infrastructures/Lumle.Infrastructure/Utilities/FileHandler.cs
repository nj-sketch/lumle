using Lumle.Infrastructure.Utilities.Abstracts;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using NLog;
using Lumle.Infrastructure.Constants.LumleLog;
using System.Text.RegularExpressions;
using ImageSharp;
using ImageSharp.PixelFormats;

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

                var destinationImage = Crop(image, width, height);

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

        private Image<Rgba32> Crop(Image sourceImage, int width, int height)
        {
            var options = new ImageSharp.Processing.ResizeOptions
            {
                Size = new Size(width, height)
            };

            return sourceImage.Resize(options);

        }

        private string RemoveSpace(string userInput)
        {
            var finalValue = "";
            try
            {
                string[] Split = userInput.Split(new Char[] { ' ' });
                //SHOW RESULT
                for (int i = 0; i < Split.Length; i++)
                {
                    finalValue += Convert.ToString(Split[i]);
                }
                finalValue = Regex.Replace(finalValue, @"[^a-zA-Z\d]+(?!(?<=\.)\w+$)", "-");
                return finalValue;
            }
            catch (Exception)
            {
                return finalValue;
            }
        }
    }
}
