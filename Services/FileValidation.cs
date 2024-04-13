using Microsoft.AspNetCore.Components.Forms;

namespace BookShelfXChange.Services
{

    public class FileValidator
    {
        public const int MaxFileSize = 4 * 1024 * 1024; // 4 MB

        public static async Task<string?> ValidateFileAsync(IBrowserFile file)
        {
            if (file == null || file.Size == 0)
            {
                return "File is empty.";
            }

            if (file.Size > MaxFileSize)
            {
                return "File size exceeds the maximum allowed size (4 MB).";
            }

            if (!IsFileValidFormat(file))
            {
                return "File type not alllowed.";
            }

            return null; // File is valid
        }

        private static bool IsFileValidFormat(IBrowserFile file)
        {
            var extension = Path.GetExtension(file.Name)?.ToLower();
            return extension == ".png" || extension == ".jpg" || extension == ".jpeg";
        }
    }

}
