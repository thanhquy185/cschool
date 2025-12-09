using Services;

namespace ViewModels;

public partial class AttendanceViewModel : ViewModelBase
{
    public string Content { get; set; } = "Trang thông tin điểm danh";

    public AttendanceViewModel()
    {
        var currentUserLogin = SessionService.currentUserLogin;
        Console.WriteLine("Trang điểm danh: " + currentUserLogin?.Username);
    }
}