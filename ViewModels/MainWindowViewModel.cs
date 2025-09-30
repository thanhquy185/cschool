using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace cschool.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // Các thuộc tính...
    // - Biến giữ trạng thái trang hiện tại
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StudentButtonActive))]
    [NotifyPropertyChangedFor(nameof(TeacherButtonActive))]
    [NotifyPropertyChangedFor(nameof(UserButtonActive))]
    private ViewModelBase _currentPage;
    // - Các biến giữ thông tin các nút (hình ảnh, nhãn dán)
    // -- Học sinh
    public Bitmap StudentButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new System.Uri("avares://cschool/Assets/Images/Others/student-icon.png")));
    public string StudentButtonLabel { get; } = "Học sinh";
    // -- Giáo viên
    public Bitmap TeacherButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new System.Uri("avares://cschool/Assets/Images/Others/teacher-icon.png")));
    public string TeacherButtonLabel { get; } = "Giáo viên";
    // -- Tài khoản
    public Bitmap UserButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new System.Uri("avares://cschool/Assets/Images/Others/user-icon.png")));
    public string UserButtonLabel { get; } = "Người dùng";
    // - Các biến models
    private readonly StudentViewModel _studentViewModel = new();
    private readonly TeacherViewModel _teacherViewModel = new();
    private readonly UserViewModel _userViewModel = new();
    // Các biến giữ trạng thái thể hiện nút được nhấn hoặc không được nhấn
    public bool StudentButtonActive => this.CurrentPage == this._studentViewModel;
    public bool TeacherButtonActive => this.CurrentPage == this._teacherViewModel;
    public bool UserButtonActive => this.CurrentPage == this._userViewModel;

    // Khởi tạo mặc định là trang Thông tin học sinh
    public MainWindowViewModel()
    {
        // Mặc định trang người dùng sẽ hiện
         _userViewModel = new UserViewModel();
        this.CurrentPage = this._userViewModel;
    }

    // Chuyển đến trang Thông tin học sinh
    [RelayCommand]
    public void GoToStudentView() => this.CurrentPage = this._studentViewModel;
    // Chuyển đến trang Thông tin giáo viên
    [RelayCommand]
    public void GoToTeacherView() => this.CurrentPage = this._teacherViewModel;
    // Chuyển đến trang Thông tin người dùng
    [RelayCommand]
    public void GoToUserView() => this.CurrentPage = this._userViewModel;
}
