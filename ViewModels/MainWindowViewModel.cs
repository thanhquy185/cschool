using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services;

namespace ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // - Người dùng đăng nhập hiện tại
    [ObservableProperty]
    private UserModel? _currentUser;
    // - Biến kiểm tra có tài khoản đang đăng nhập ?
    [ObservableProperty]
    private bool _isLoggedIn = false;
    // - Trang hiện tại
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatisticalButtonActive))]
    [NotifyPropertyChangedFor(nameof(AssignTeacherButtonActive))]
    [NotifyPropertyChangedFor(nameof(ExamButtonActive))]
    [NotifyPropertyChangedFor(nameof(TuitionButtonActive))]
    [NotifyPropertyChangedFor(nameof(HomeClassButtonActive))]
    [NotifyPropertyChangedFor(nameof(AttendanceButtonActive))]
    [NotifyPropertyChangedFor(nameof(SubjectClassButtonActive))]
    [NotifyPropertyChangedFor(nameof(TeacherButtonActive))]
    [NotifyPropertyChangedFor(nameof(StudentButtonActive))]
    [NotifyPropertyChangedFor(nameof(RoleButtonActive))]
    [NotifyPropertyChangedFor(nameof(UserButtonActive))]
    private ViewModelBase _currentPage;

    // - Các biến giữ thông tin các nút (hình ảnh, nhãn dán)
    // -- Thống kê
    public Bitmap StatisticalButtonImage { get; }
  = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/analytics.png")));
    public string StatisticalButtonLabel => "Thống kê ";
    // -- Phân công giáo viên
    public Bitmap AssignTeacherButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/subject-icon.png")));
    public string AssignTeacherButtonLabel => "Phân công giáo viên";
    // -- Lịch thi
    public Bitmap ExamButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/exam-schedule-icon.png")));
    public string ExamButtonLabel { get; } = "Lịch thi";
    // -- Học phí
    public Bitmap TuitionButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/tuition-icon.png")));
    public string TuitionButtonLabel { get; } = "Học phí";
    // -- Lớp chủ nhiệm
    public Bitmap HomeClassButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/business-persentation.png")));
    public string HomeClassButtonLabel => "Lớp chủ nhiệm";
    // -- Điểm danh
    public Bitmap AttendanceButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/attendance-icon.png")));
    public string AttendanceButtonLabel { get; } = "Điểm danh";
    // -- Lớp môn học
    public Bitmap SubjectClassButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/scoreboard.png")));
    public string SubjectClassButtonLabel => "Nhập điểm lớp môn học";
    // -- Giáo viên
    public Bitmap TeacherButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/teacher-icon.png")));
    public string TeacherButtonLabel { get; } = "Giáo viên";
    // -- Học sinh
    public Bitmap StudentButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/student-icon.png")));
    public string StudentButtonLabel { get; } = "Học sinh";
    // -- Nhóm quyền
    public Bitmap RoleButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/role-icon.png")));
    public string RoleButtonLabel { get; } = "Nhóm quyền";
    // -- Người dùng
    public Bitmap UserButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/user-icon.png")));
    public string UserButtonLabel { get; } = "Người dùng";

    // - Các biến models
    private readonly LoginViewModel _loginViewModel = new();
    private readonly StatisticalViewModel _statisticalViewModel;
    private readonly AssignTeacherViewModel _assignTeacherViewModel;
    private readonly ExamViewModel _examViewModel = new();
    private readonly TuitionViewModel _tuitionViewModel = new();
    private readonly HomeClassViewModel _homeClassViewModel;
    private readonly SubjectClassViewModel _subjectClassViewModel = new();
    private readonly AttendanceViewModel _attendanceViewModel = new();
    private readonly TeacherViewModel _teacherViewModel = new();
    private readonly StudentViewModel _studentViewModel = new();
    private readonly RoleViewModel _roleViewModel = new();
    private readonly UserViewModel _userViewModel = new();

    // Các biến giữ trạng thái thể hiện nút được nhấn hoặc không được nhấn
    public bool StatisticalButtonActive => this.CurrentPage == this._statisticalViewModel;
    public bool AssignTeacherButtonActive => this.CurrentPage == this._assignTeacherViewModel;
    public bool ExamButtonActive => this.CurrentPage == this._examViewModel;
    public bool TuitionButtonActive => this.CurrentPage == this._tuitionViewModel;
    public bool HomeClassButtonActive => this.CurrentPage == this._homeClassViewModel;
    public bool AttendanceButtonActive => this.CurrentPage == this._attendanceViewModel;
    public bool SubjectClassButtonActive => this.CurrentPage == this._subjectClassViewModel;
    public bool TeacherButtonActive => this.CurrentPage == this._teacherViewModel;
    public bool StudentButtonActive => this.CurrentPage == this._studentViewModel;
    public bool RoleButtonActive => this.CurrentPage == this._roleViewModel;
    public bool UserButtonActive => this.CurrentPage == this._userViewModel;


    // Constructor
    public MainWindowViewModel()
    {
        //Khởi tạo các view model con
        // _studentViewModel = new StudentViewModel();
        // _teacherViewModel = new TeacherViewModel();
        _assignTeacherViewModel = new AssignTeacherViewModel(new AssignTeacherService(AppService.DBService));
        _subjectClassViewModel = new SubjectClassViewModel();
        _statisticalViewModel = new StatisticalViewModel(new StatisticalService(AppService.DBService));
        _homeClassViewModel = new HomeClassViewModel(new HomeClassService(AppService.DBService));

        // // Mặc định là trang đăng nhập hiển thị
        // _loginViewModel = new LoginViewModel();
        // this.CurrentPage = this._loginViewModel;

        // // Thiệt lập hàm xử lý đăng nhập
        // HandleLogin();

        _roleViewModel = new RoleViewModel();
        this.CurrentPage = this._roleViewModel;
        this.IsLoggedIn = true;
    }

    // - Hàm xử lý đăng nhập
    private void HandleLogin()
    {
        _loginViewModel.OnLoginSuccess = (user) =>
        {
            this.CurrentPage = _userViewModel;
            this.CurrentUser = user;
            this.IsLoggedIn = true;
        };
    }
    // - Hàm xử lý đăng xuất
    [RelayCommand]
    private void HandleLogout()
    {
        this.CurrentUser = null;
        this.IsLoggedIn = false;
        this.CurrentPage = _loginViewModel;
    }
    // - Chuyển đến trang Thống kê
    [RelayCommand]
    public void GoToStatisticalView() => this.CurrentPage = this._statisticalViewModel;
    // - Chuyển đến trang Phân công giáo viên
    [RelayCommand]
    public void GoToAssignTeacherView() => this.CurrentPage = this._assignTeacherViewModel;
    // - Chuyển đến trang Lịch thi
    [RelayCommand]
    public void GoToExamView() => this.CurrentPage = this._examViewModel;
    // - Chuyển đến trang Học phí
    [RelayCommand]
    public void GoToTuitionView() => this.CurrentPage = this._tuitionViewModel;
    // - Chuyển đến trang Lớp chủ nhiệm
    [RelayCommand]
    public void GoToHomeClassView() => this.CurrentPage = this._homeClassViewModel;
    // - Chuyển đến trang Điểm danh
    [RelayCommand]
    public void GoToAttendanceView() => this.CurrentPage = this._attendanceViewModel;
    // - Chuyển đến trang Nhập điểm học sinh
    [RelayCommand]
    public void GoToSubjectClassView() => this.CurrentPage = this._subjectClassViewModel;
    // - Chuyển đến trang Giáo viên
    [RelayCommand]
    public void GoToTeacherView() => this.CurrentPage = this._teacherViewModel;
    // - Chuyển đến trang Học sinh
    [RelayCommand]
    public void GoToStudentView() => this.CurrentPage = this._studentViewModel;
    // - Chuyển đến trang Nhóm quyền
    [RelayCommand]
    public void GoToRoleView() => this.CurrentPage = this._roleViewModel;
    // - Chuyển đến trang Người dùng
    [RelayCommand]
    public void GoToUserView() => this.CurrentPage = this._userViewModel;
}
