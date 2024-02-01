namespace Videos.Application.Services
{
    public static class LocalFileService
    {
        public static async Task<FileInfo> SaveFileAsync(Stream stream, string originalName, int userId)
        {
            var uploadPath = GetUploadDir(userId);
            var fullName = Path.Combine(uploadPath, GenerateFileName(originalName));


            using (var fileStream = new FileStream(fullName, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            stream.Close();
            return new FileInfo(fullName);
        }

        public static string GenerateFileName(string fileName)
        {
            return Path.GetRandomFileName() + "_" + fileName;
        }

        public static string GetUploadDir(int userId)
        {
            var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "UploadedFiles", userId.ToString()));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetThumbailDir(int userId)
        {
            var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "UploadedThumbails", userId.ToString()));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
