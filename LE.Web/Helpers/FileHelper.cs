using Microsoft.AspNetCore.Http;

namespace LE.Web.Helpers
{
    public interface FileHelper
    {
        bool isImageValid(string file_name);
        string saveImageAndGetFileName(IFormFile file, string file_prefix = "");
        bool isExcelFileValid(string file_name);
        bool isImageSizeLessThan1Mb(IFormFile file);
    }
}
