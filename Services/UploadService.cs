
public static class UploadService
{
    /// <summary>
    /// Upload một file ảnh vào thư mục chung theo objectType + id.
    /// </summary>
    /// <param name="sourcePath">Đường dẫn file gốc</param>
    /// <param name="objectType">Loại đối tượng (vd: "user", "student")</param>
    /// <param name="objectId">Id của đối tượng</param>
    /// <returns>Tên file được lưu (để ghi DB)</returns>
    public static async Task<string?> SaveImageAsync(string sourcePath, string objectType, int objectId)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            return null;

        // Lấy extension
        var ext = Path.GetExtension(sourcePath);

        // Tạo tên file mới
        var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{objectType}-{objectId}{ext}";

        // Đường dẫn thư mục đích
        var destDir = Path.Combine(AppService.AppPath, "Assets", "Images", Capitalize($"{objectType}s"));
        Console.WriteLine(destDir);
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        // Đường dẫn file đích
        var destPath = Path.Combine(destDir, fileName);

        // Copy (nếu lớn có thể dùng FileStream async)
        using (var sourceStream = File.OpenRead(sourcePath))
        using (var destStream = File.Create(destPath))
        {
            await sourceStream.CopyToAsync(destStream);
        }

        return fileName; // chỉ lưu tên file vào DB
    }

    /// <summary>
    /// Load ảnh từ tên file (avatar, photo…) theo objectType
    /// </summary>
    // public static Bitmap? LoadImage(string fileName, string objectType)
    // {
    //     if (string.IsNullOrWhiteSpace(fileName))
    //         return null;

    //     var filePath = Path.Combine(AppService.AppPath, "Assets", "Images", Capitalize(objectType), fileName);
    //     return File.Exists(filePath) ? new Bitmap(filePath) : null;
    // }

    private static string Capitalize(string str) =>
        string.IsNullOrWhiteSpace(str) ? str : char.ToUpper(str[0]) + str.Substring(1).ToLower();
}
