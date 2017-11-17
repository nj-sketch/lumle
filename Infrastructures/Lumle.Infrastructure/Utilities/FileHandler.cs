using Lumle.Infrastructure.Utilities.Abstracts;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using NLog;
using Lumle.Infrastructure.Constants.LumleLog;
using SixLabors.ImageSharp;
using SixLabors.Primitives;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml;

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

        public async Task<string> SaveFile(byte[] bytes, string fileName, string storePath, string suffix = null)
        {
            suffix = suffix ?? DateTime.UtcNow.Ticks.ToString();

            string ext = Path.GetExtension(fileName);
            string name = Path.GetFileNameWithoutExtension(fileName);
            string folder = Path.Combine(_environment.WebRootPath, storePath);
            string relative = $"files/{name}_{suffix}{ext}";
            string absolute = Path.Combine(folder, relative);
            string dir = Path.GetDirectoryName(absolute);

            Directory.CreateDirectory(dir);
            using (var writer = new FileStream(absolute, FileMode.CreateNew))
            {
                await writer.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }

            return "/" + storePath + "/" + relative;
        }

        public async Task<string> SaveImageFromEditor(string content, string storePath)
        {
            var imgRegex = new Regex("<img[^>].+ />", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var base64Regex = new Regex("data:[^/]+/(?<ext>[a-z]+);base64,(?<base64>.+)", RegexOptions.IgnoreCase);

            foreach (Match match in imgRegex.Matches(content))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<root>" + match.Value + "</root>");

                var img = doc.FirstChild.FirstChild;
                var srcNode = img.Attributes["src"];
                var fileNameNode = img.Attributes["data-filename"];

                // The HTML editor creates base64 DataURIs which we'll have to convert to image files on disk
                if (srcNode != null && fileNameNode != null)
                {
                    var base64Match = base64Regex.Match(srcNode.Value);
                    if (base64Match.Success)
                    {
                        byte[] bytes = Convert.FromBase64String(base64Match.Groups["base64"].Value);
                        srcNode.Value = await SaveFile(bytes, fileNameNode.Value, storePath).ConfigureAwait(false);

                        img.Attributes.Remove(fileNameNode);
                        content = content.Replace(match.Value, img.OuterXml);
                    }
                }
            }

            return content;
        }
    }
}
