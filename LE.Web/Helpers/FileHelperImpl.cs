using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LE.Web.Helpers
{
    public class FileHelperImpl : FileHelper
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileHelperImpl(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public bool isImageValid(string file_name)
        {
            var allowedExtensions = new[] { ".jpeg", ".png", ".jpg" };
            var extension = Path.GetExtension(file_name).ToLower();
            if (!allowedExtensions.Contains(extension))
                return false;
            return true;
        }

        public bool isExcelFileValid(string file_name)
        {
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var extension = Path.GetExtension(file_name).ToLower();
            if (!allowedExtensions.Contains(extension))
                return false;
            return true;
        }

        public string saveImageAndGetFileName(IFormFile file, string file_prefix = "")
        {
            if (!isImageValid(file.FileName))
            {
                throw new LE.Web.Exceptions.CustomException("invalid Document format. Document must be an image.");
            }

            if (!isImageSizeLessThan1Mb(file))
                throw new LE.Web.Exceptions.CustomException("Image size must be less than 10 Mb.");
            Random random = new Random();
            string plainFilePrefix = Regex.Replace(file_prefix, "[:!@#+*/=$%^&*()}{|\":?><\\[\\]\\;/.,~]", "");
            string file_name = "";
            if (string.IsNullOrWhiteSpace(file_prefix))
            {
                file_name = Path.GetFileNameWithoutExtension(file.FileName) + random.Next(1, 1232384943) + Path.GetExtension(file.FileName);
            }
            else
            {
                file_name = plainFilePrefix + random.Next(1, 1232384943) + Path.GetExtension(file.FileName);
            }

            // The upload folder is git-ignored, so fresh clones/deployments lack
            // it — ensure it exists or every image save throws DirectoryNotFound.
            var uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "images/custom");
            Directory.CreateDirectory(uploadDir);
            var filePath = Path.Combine(uploadDir, file_name);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                using (MagickImage magickImage = new MagickImage(file.OpenReadStream()))
                {
                    magickImage.Format = MagickFormat.Jpeg;
                    magickImage.SetCompression(CompressionMethod.LosslessJPEG);
                    magickImage.Quality = 30;
                    magickImage.Write(stream);
                }
            }

            return file_name;
        }

        public bool isImageSizeLessThan1Mb(IFormFile file)
        {
            if (file != null)
            {
                int maxFileSize = 1024 * 1024 * 10;
                if (file.Length <= maxFileSize)
                    return true;
            }
            return false;
        }
    }
}
