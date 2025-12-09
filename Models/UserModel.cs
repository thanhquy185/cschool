using Avalonia.Media.Imaging;
using Avalonia.Platform;

public class UserModel
{
    // Properties
    // - Cột trong bảng dữ liệu
    public int Id { get; set; }
    public string Avatar { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public string Fullname { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Status { get; set; }
    // - Giúp lưu trữ đường dẫn ảnh khi thêm và cập nhật
    public string AvatarFile { get; set; }
    // - Giúp chuyển đổi thành ảnh kiểu bitmap khi hiển thị trên giao diện
    public Bitmap AvatarImage
    {
        get
        {
            var uri = new Uri($"avares://Views/Assets/Images/Others/no-image.png");
            if (!string.IsNullOrEmpty(Avatar))
            {
                uri = new Uri($"avares://Views/Assets/Images/Users/{Avatar}");
            }
            return new Bitmap(AssetLoader.Open(uri));
        }
    }

    // Constructors
    public UserModel() { }
    public UserModel(int Id, string Avatar, string Username, string Password,
        int RoleId, string Fullname, string Phone, string Email,
        string Address, string Status)
    {
        this.Id = Id;
        this.Avatar = Avatar;
        this.Username = Username;
        this.Password = Password;
        this.RoleId = RoleId;
        this.Fullname = Fullname;
        this.Phone = Phone;
        this.Email = Email;
        this.Address = Address;
        this.Status = Status;
    }
}
