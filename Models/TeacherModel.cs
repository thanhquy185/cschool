using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.IO;
using System.Collections.Generic;
namespace cschool.Models;

public class TeacherModel
{
    public int Id { get; set; }
    public string Birthday { get; set; } = "";
    public string Avatar { get; set; } = "";
    public string AvatarFile { get; set; } = "";
    public string Gender { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public int ClassId { get; set; } = 0;
    public string ClassName { get; set; } = "";
    public int DepartmentId { get; set; } = 0;
    public string DepartmentName { get; set; } = "";
    public int Status { get; set; } = 1;
    public string TermName { get; set; } = "";
    public string StatusText => Status == 1 ? "Hoạt động" : "Ẩn";
    public Bitmap AvatarImage
    {
        get
        {
            try
            {
                // Nếu người dùng đã chọn ảnh từ ổ cứng
                if (!string.IsNullOrEmpty(AvatarFile) && File.Exists(AvatarFile))
                {
                    return new Bitmap(AvatarFile);
                }

                // Nếu ảnh đã lưu trong thư mục Assets/Images/Students
                if (!string.IsNullOrEmpty(Avatar))
                {
                    // Xác định đường dẫn tuyệt đối đến ảnh
                    var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
                    var imagePath = Path.Combine(projectRoot, "Assets", "Images", "Teachers", Avatar);
                    Console.WriteLine("Đường dẫn ảnh giáo viên: " + imagePath);

                    if (File.Exists(imagePath))
                        return new Bitmap(imagePath);
                }

                // Nếu không có ảnh → dùng ảnh mặc định
                var defaultUri = new Uri($"avares://cschool/Assets/Images/Others/no-image.png");
                return new Bitmap(AssetLoader.Open(defaultUri));
            }
            catch
            {
                // Nếu xảy ra lỗi → fallback sang ảnh mặc định
                var defaultUri = new Uri($"avares://cschool/Assets/Images/Others/no-image.png");
                return new Bitmap(AssetLoader.Open(defaultUri));
            }
        }
    }
    public DateTimeOffset? BirthdayDate
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Birthday))
                return null;

            try
            {
                // Parse từ nhiều format
                if (DateTime.TryParseExact(Birthday, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date1))
                    return new DateTimeOffset(date1);
                
                if (DateTime.TryParse(Birthday, out var date3))
                    return new DateTimeOffset(date3);

                return null;
            }
            catch
            {
                return null;
            }
        }
        set
        {
            if (value.HasValue)
            {
                Birthday = value.Value.DateTime.ToString("dd/MM/yyyy"); // Format cho database
            }
            else
            {
                Birthday = string.Empty;
            }
        }
    }
}