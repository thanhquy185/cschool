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
    public UserModel? _currentUserLogin;
    // - Biến kiểm tra có tài khoản đang đăng nhập ?
    [ObservableProperty]
    private bool _isLoggedIn = false;
    // - Trang hiện tại
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AttendanceButtonActive))]
    [NotifyPropertyChangedFor(nameof(SubjectClassButtonActive))]
    [NotifyPropertyChangedFor(nameof(ClassButtonActive))]
    [NotifyPropertyChangedFor(nameof(StatisticalButtonActive))]
    [NotifyPropertyChangedFor(nameof(AssignTeacherButtonActive))]
    [NotifyPropertyChangedFor(nameof(ExamButtonActive))]
    [NotifyPropertyChangedFor(nameof(TuitionButtonActive))]
    [NotifyPropertyChangedFor(nameof(HomeClassButtonActive))]
    [NotifyPropertyChangedFor(nameof(TeacherButtonActive))]
    [NotifyPropertyChangedFor(nameof(StudentButtonActive))]
    [NotifyPropertyChangedFor(nameof(RoleButtonActive))]
    [NotifyPropertyChangedFor(nameof(UserButtonActive))]
    private ViewModelBase _currentPage;

    // - Các biến giữ thông tin các nút (hình ảnh, nhãn dán)
    // -- Lớp chủ nhiệm
    public Bitmap HomeClassButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/business-persentation.png")));
    public string HomeClassButtonLabel => "Lớp chủ nhiệm";
    // -- Điểm danh
    // public Bitmap AttendanceButtonImage { get; }
    //     = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/attendance-icon.png")));
    // public string AttendanceButtonLabel { get; } = "Điểm danh";
    // -- Lớp môn học
    public Bitmap SubjectClassButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/scoreboard.png")));
    public string SubjectClassButtonLabel => "Nhập điểm";
    // -- Thống kê
    public Bitmap StatisticalButtonImage { get; }
  = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/analytics.png")));
    public string StatisticalButtonLabel => "Thống kê ";
    // -- Lịch thi
    public Bitmap AssignTeacherButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/subject-icon.png")));
    public string AssignTeacherButtonLabel { get; } = "Phân công giáo viên";
    // -- Lịch thi
    public Bitmap ExamButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/exam-schedule-icon.png")));
    public string ExamButtonLabel { get; } = "Lịch thi";
    // -- Học phí
    public Bitmap TuitionButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/tuition-icon.png")));
    public string TuitionButtonLabel { get; } = "Học phí";
    // -- Lớp học
    public Bitmap ClassButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/class-icon.png")));
    public string ClassButtonLabel { get; } = "Lớp học";
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
    private HomeClassViewModel _homeClassViewModel  = new();
    private readonly AttendanceViewModel _attendanceViewModel  = new();
    private  SubjectClassViewModel _subjectClassViewModel  = new();
    private StatisticalViewModel _statisticalViewModel  = new();
    private AssignTeacherViewModel _assignTeacherViewModel  = new();
    private ExamViewModel _examViewModel  = new();
    private TuitionViewModel _tuitionViewModel = new();
    private  ClassViewModel _classViewModel = new();
    private TeacherViewModel _teacherViewModel = new();
    private StudentViewModel _studentViewModel = new();
    private readonly RoleViewModel _roleViewModel = new ();
    private UserViewModel _userViewModel  = new();

    // Các biến giữ trạng thái thể hiện nút được nhấn hoặc không được nhấn
    public bool HomeClassButtonActive => this.CurrentPage == this._homeClassViewModel;
    public bool AttendanceButtonActive => this.CurrentPage == this._attendanceViewModel;
    public bool SubjectClassButtonActive => this.CurrentPage == this._subjectClassViewModel;
    public bool StatisticalButtonActive => this.CurrentPage == this._statisticalViewModel;
    public bool AssignTeacherButtonActive => this.CurrentPage == this._assignTeacherViewModel;
    public bool ExamButtonActive => this.CurrentPage == this._examViewModel;
    public bool TuitionButtonActive => this.CurrentPage == this._tuitionViewModel;
    public bool ClassButtonActive => this.CurrentPage == this._classViewModel;
    public bool TeacherButtonActive => this.CurrentPage == this._teacherViewModel;
    public bool StudentButtonActive => this.CurrentPage == this._studentViewModel;
    public bool RoleButtonActive => this.CurrentPage == this._roleViewModel;
    public bool UserButtonActive => this.CurrentPage == this._userViewModel;

    // - Các biến để phân quyền linh động (Nếu có chức năng mà "Xem" thì hiện)
    [ObservableProperty]
    public bool _homeClassButtonVisible;
    [ObservableProperty]
    public bool _attendanceButtonVisible;
    [ObservableProperty]
    public bool _subjectClassButtonVisible;
    [ObservableProperty]
    public bool _statisticalButtonVisible;
    [ObservableProperty]
    public bool _assignTeacherButtonVisible;
    [ObservableProperty]
    public bool _examButtonVisible;
    [ObservableProperty]
    public bool _tuitionButtonVisible;
    [ObservableProperty]
    public bool _classButtonVisible;
    [ObservableProperty]
    public bool _teacherButtonVisible;
    [ObservableProperty]
    public bool _studentButtonVisible;
    [ObservableProperty]
    public bool _roleButtonVisible;
    [ObservableProperty]
    public bool _userButtonVisible;

    // Constructor
    public MainWindowViewModel()
    {
        // Mặc định là trang đăng nhập hiển thị
        _loginViewModel = new LoginViewModel();
        this.CurrentPage = this._loginViewModel;

        // Thiệt lập hàm xử lý đăng nhập
        HandleLogin();
    }

    // - Hàm xử lý đăng nhập
    private void HandleLogin()
    {
        _loginViewModel.OnLoginSuccess = (user) =>
        {
            SessionService.currentUserLogin = user;

            this.IsLoggedIn = true;
            this.CurrentUserLogin = user;

            _homeClassViewModel = new HomeClassViewModel(user.Teacher_id??0);
            _subjectClassViewModel = new SubjectClassViewModel(user.Teacher_id??0);

            if (this.CurrentUserLogin != null && IsLoggedIn)
            {
                this.UserButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.User, "Xem");
                if (this.UserButtonVisible) this.CurrentPage = this._userViewModel;

                this.RoleButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.Role, "Xem");
                if (this.RoleButtonVisible) this.CurrentPage = this._roleViewModel;

                this.StudentButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.Student, "Xem");
                if (this.StudentButtonVisible) this.CurrentPage = this._studentViewModel;

                this.TeacherButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.Teacher, "Xem");
                if (this.TeacherButtonVisible) this.CurrentPage = this._teacherViewModel;

                this.ClassButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.Class, "Xem");
                if (this.ClassButtonVisible) this.CurrentPage = this._classViewModel;

                this.TuitionButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.Tuition, "Xem");
                if (this.TuitionButtonVisible) this.CurrentPage = this._tuitionViewModel;

                this.ExamButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.Exam, "Xem");
                if (this.ExamButtonVisible) this.CurrentPage = this._examViewModel;

                this.AssignTeacherButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.AssignTeacher, "Xem");
                if (this.AssignTeacherButtonVisible) this.CurrentPage = this._assignTeacherViewModel;

                this.StatisticalButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.Statistical, "Xem");
                if (this.StatisticalButtonVisible) this.CurrentPage = this._statisticalViewModel;

                this.SubjectClassButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.SubjectClass, "");
                if (this.SubjectClassButtonVisible) this.CurrentPage = this._subjectClassViewModel;

                this.AttendanceButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId, (int)FunctionIdEnum.Attendance, "");
                if (this.AttendanceButtonVisible) this.CurrentPage = this._attendanceViewModel;

                this.HomeClassButtonVisible = AppService.RoleDetailService.HasPermission(
                    this.CurrentUserLogin.RoleId,(int)FunctionIdEnum.HomeClass, "");
                if (this.HomeClassButtonVisible)
                    this.CurrentPage = this._homeClassViewModel;
                    
                 
            }
        };
    }

    // - Hàm xử lý đăng xuất
    [RelayCommand]
    private void HandleLogout()
    {
        this.CurrentUserLogin = null;
        this.IsLoggedIn = false;
        this.CurrentPage = _loginViewModel;
    }
    // - Chuyển đến trang Lớp chủ nhiệm
    [RelayCommand]
    public void GoToHomeClassView()
    {
        this._homeClassViewModel = new HomeClassViewModel();
        this.CurrentPage = this._homeClassViewModel;
    }
    // - Chuyển đến trang Điểm danh
    [RelayCommand]
    public void GoToAttendanceView() => this.CurrentPage = this._attendanceViewModel;
    // - Chuyển đến trang Nhập điểm học sinh
    [RelayCommand]
    public void GoToSubjectClassView()
    {
        this._subjectClassViewModel = new SubjectClassViewModel();
        this.CurrentPage = this._subjectClassViewModel;
    }
    // - Chuyển đến trang Thống kê
    [RelayCommand]
    public void GoToStatisticalView()
    {
        this._statisticalViewModel = new StatisticalViewModel();
        this.CurrentPage = this._statisticalViewModel;
    }
    // - Chuyển đến trang Phân công giáo viên
    [RelayCommand]
    public void GoToAssignTeacherView()
    {
        this._assignTeacherViewModel = new AssignTeacherViewModel();
        this.CurrentPage = this._assignTeacherViewModel;
    }
    // - Chuyển đến trang Lịch thi
    [RelayCommand]
    public void GoToExamView()
    {
        this._examViewModel = new ExamViewModel();
        this.CurrentPage = this._examViewModel;
    }
    // - Chuyển đến trang Học phí
    [RelayCommand]
    public void GoToTuitionView()
    {
        this._tuitionViewModel = new TuitionViewModel();
        this.CurrentPage = this._tuitionViewModel;
    }
    // - Chuyển đến trang Lớp học
    [RelayCommand]
    public void GoToClassView()
    {
        this._classViewModel = new ClassViewModel();
        this.CurrentPage = this._classViewModel;
    }
    // - Chuyển đến trang Giáo viên
    [RelayCommand]
    public void GoToTeacherView() {
        this._teacherViewModel = new TeacherViewModel();
        this.CurrentPage = this._teacherViewModel;
    }
    // - Chuyển đến trang Học sinh
    [RelayCommand]
    public void GoToStudentView()
    {
        this._studentViewModel = new StudentViewModel();
        this.CurrentPage = this._studentViewModel;
    }
    // - Chuyển đến trang Nhóm quyền
    [RelayCommand]
    public void GoToRoleView() => this.CurrentPage = this._roleViewModel;
    // - Chuyển đến trang Người dùng
    [RelayCommand]
    public void GoToUserView()
    {
        this._userViewModel = new UserViewModel();
        this.CurrentPage = this._userViewModel;
    }

}
