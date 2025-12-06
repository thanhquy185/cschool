using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.IO;

public class StudentModel
{
    public int Id { get; set; }
    public string Avatar { get; set; }
    public string Fullname { get; set; }
    public string BirthDay { get; set; }
    public string Gender { get; set; }
    public string Ethnicity { get; set; }
    public string Religion { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string LearnYear { get; set; }
    public string LearnStatus { get; set; }
    public string ClassName { get; set; }
    public string ClassYear { get; set; }
    public string Name { get; set; }
    public sbyte Status { get; set; }
    public string Role { get; set; }
    // - Giúp lưu trữ đường dẫn ảnh khi thêm và cập nhật
    public string AvatarFile { get; set; }
    // - Giúp chuyển đổi thành ảnh kiểu bitmap khi hiển thị trên giao diện
    // Dùng cho hiển thị ảnh trong Avalonia UI
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
                    var imagePath = Path.Combine(projectRoot, "Assets", "Images", "Students", Avatar);

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
    public string BirthDayDisplay
    {
        get
        {
            if (DateTime.TryParse(BirthDay, out var date))
                return date.ToString("dd/MM/yyyy");
            return BirthDay;
        }
    }
}
