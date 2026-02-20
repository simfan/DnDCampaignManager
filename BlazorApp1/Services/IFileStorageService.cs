namespace BlazorApp1.Services
{
    public interface IFileStorageService
    {
        Task<(string filePath, string fileName)> SaveFileAsync(IFormFile file, int campaignId);
        Task<bool> DeleteFileAsync(string filePath);
        Task<(byte[] fileData, string contentType)> GetFileAsync(string filePath);
        string GetFileUrl(string filePath);
    }

    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadPath;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _uploadPath = Path.Combine(_environment.ContentRootPath, "Uploads", "Resources");

            if(!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<(string filePath, string fileName)> SaveFileAsync(IFormFile file, int campaignId)
        {
            if(file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty");
            }

            //Create Directory
            var campaignPath = Path.Combine(_uploadPath, $"Campaign_{campaignId}");
            if (!Directory.Exists(campaignPath))
            {
                Directory.CreateDirectory(campaignPath);
            }

            //Generate Unique Filename
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var fullPath = Path.Combine(campaignPath, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //return relative path for database storage
            var relativePath = Path.Combine("Resources", $"Campaign_{campaignId}", uniqueFileName);
            return (relativePath, file.FileName);
        }

        public Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_uploadPath, "..", filePath);
                if(File.Exists(fullPath))
                {
                    
                    File.Delete(fullPath);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<(byte[] fileData, string contentType)> GetFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_uploadPath, "..", filePath);

            if(!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found");
            }

            var fileData = await File.ReadAllBytesAsync(fullPath);
            var contentType = GetContentType(filePath);

            return(fileData, contentType);
        }

        public string GetFileUrl(string filePath)
        {
            return $"/api/resources/file/{filePath}";
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                _ => "application/octet-stream"
            };
        }
    }
}
