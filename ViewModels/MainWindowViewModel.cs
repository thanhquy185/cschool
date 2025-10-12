using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using cschool.Services;
using System;

namespace cschool.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // - Trang hiện tại
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StudentButtonActive))]
    [NotifyPropertyChangedFor(nameof(TeacherButtonActive))]
    [NotifyPropertyChangedFor(nameof(UserButtonActive))]
    [NotifyPropertyChangedFor(nameof(AssignTeacherButtonActive))]
    private ViewModelBase _currentPage;

    // - Các ViewModel con
    private readonly StudentViewModel _studentViewModel;
    private readonly TeacherViewModel _teacherViewModel;
    private readonly UserViewModel _userViewModel;
    private readonly AssignTeacherViewModel _assignTeacherViewModel;

    // - Hình ảnh và nhãn menu
    public Bitmap StudentButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://cschool/Assets/Images/Others/student-icon.png")));
    public string StudentButtonLabel => "Học sinh";

    public Bitmap TeacherButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://cschool/Assets/Images/Others/teacher-icon.png")));
    public string TeacherButtonLabel => "Giáo viên";

    public Bitmap UserButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://cschool/Assets/Images/Others/user-icon.png")));
    public string UserButtonLabel => "Người dùng";

public Bitmap AssignTeacherButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://cschool/Assets/Images/Others/subject-icon.png")));
    public string AssignTeacherButtonLabel => "Phân công giáo viên";

    // - Trạng thái nút active
    public bool StudentButtonActive => CurrentPage == _studentViewModel;
    public bool TeacherButtonActive => CurrentPage == _teacherViewModel;
    public bool UserButtonActive => CurrentPage == _userViewModel;
    public bool AssignTeacherButtonActive => CurrentPage == _assignTeacherViewModel;

    // Constructor
    public MainWindowViewModel()
    {
        //Khởi tạo các view model con
        _studentViewModel = new StudentViewModel();
        _teacherViewModel = new TeacherViewModel();
        _userViewModel = new UserViewModel();
         _assignTeacherViewModel = new AssignTeacherViewModel(new AssignTeacherService(AppService.DBService));
        // Trang mặc định hiển thị
        CurrentPage = _userViewModel;
    }

    // - Chuyển trang
    [RelayCommand]
    public void GoToStudentView() => CurrentPage = _studentViewModel;

    [RelayCommand]
    public void GoToTeacherView() => CurrentPage = _teacherViewModel;

    [RelayCommand]
    public void GoToUserView() => CurrentPage = _userViewModel;

    [RelayCommand]
    public void GoToAssignTeacherView() => CurrentPage = _assignTeacherViewModel;
}
