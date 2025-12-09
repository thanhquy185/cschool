using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services;

namespace ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // - Trang hiện tại
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StudentButtonActive))]
    [NotifyPropertyChangedFor(nameof(TeacherButtonActive))]
    [NotifyPropertyChangedFor(nameof(UserButtonActive))]
    [NotifyPropertyChangedFor(nameof(ExamButtonActive))]
    [NotifyPropertyChangedFor(nameof(TuitionButtonActive))]
    [NotifyPropertyChangedFor(nameof(AttendanceButtonActive))]
    [NotifyPropertyChangedFor(nameof(ClassButtonActive))]
    [NotifyPropertyChangedFor(nameof(AssignTeacherButtonActive))]
    [NotifyPropertyChangedFor(nameof(SubjectClassButtonActive))]
    [NotifyPropertyChangedFor(nameof(StatisticalButtonActive))]
    [NotifyPropertyChangedFor(nameof(HomeClassButtonActive))]
    private ViewModelBase _currentPage;
    // - Các biến giữ thông tin các nút (hình ảnh, nhãn dán)

    // -- Học sinh
    public Bitmap StudentButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/student-icon.png")));
    public string StudentButtonLabel { get; } = "Học sinh";

    // -- Giáo viên
    public Bitmap TeacherButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/teacher-icon.png")));
    public string TeacherButtonLabel { get; } = "Giáo viên";

    // -- Tài khoản
    public Bitmap UserButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/user-icon.png")));
    public string UserButtonLabel { get; } = "Người dùng";

    // -- Lịch thi
    public Bitmap ExamButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/exam-schedule-icon.png")));
    public string ExamButtonLabel { get; } = "Lịch thi";

    // -- Học phí
    public Bitmap TuitionButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/tuition-icon.png")));
    public string TuitionButtonLabel { get; } = "Học phí";

    // -- Điểm danh
    public Bitmap AttendanceButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/attendance-icon.png")));
    public string AttendanceButtonLabel { get; } = "Điểm danh";

    // Lớp học
    public Bitmap ClassButtonImage { get; }
        = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/class-icon.png")));
    public string ClassButtonLabel { get; } = "Lớp học";

    // -- Lớp chủ nhiệm
    public Bitmap HomeClassButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/business-persentation.png")));
    public string HomeClassButtonLabel => "Lớp chủ nhiệm";

    // -- Phân công giáo viên
    public Bitmap AssignTeacherButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/subject-icon.png")));
    public string AssignTeacherButtonLabel => "Phân công giáo viên";

    // -- Lớp môn học
    public Bitmap SubjectClassButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/scoreboard.png")));
    public string SubjectClassButtonLabel => "Nhập điểm lớp môn học";

    // -- Thống kê
      public Bitmap StatisticalButtonImage { get; }
    = new Bitmap(AssetLoader.Open(new Uri("avares://Views/Assets/Images/Others/analytics.png")));
    public string StatisticalButtonLabel => "Thống kê ";

    // - Các biến models
    private readonly StudentViewModel _studentViewModel = new();
    private readonly TeacherViewModel _teacherViewModel = new();
    private readonly UserViewModel _userViewModel = new();
    private readonly ExamViewModel _examViewModel = new();
    private readonly TuitionViewModel _tuitionViewModel = new();
    private readonly AttendanceViewModel _attendanceViewModel = new();
    private readonly ClassViewModel _classViewModel = new();
    private readonly AssignTeacherViewModel _assignTeacherViewModel;
    private readonly SubjectClassViewModel _subjectClassViewModel = new();
    private readonly StatisticalViewModel _statisticalViewModel;
    private readonly HomeClassViewModel _homeClassViewModel;

    // Các biến giữ trạng thái thể hiện nút được nhấn hoặc không được nhấn
    public bool StudentButtonActive => this.CurrentPage == this._studentViewModel;
    public bool TeacherButtonActive => this.CurrentPage == this._teacherViewModel;
    public bool UserButtonActive => this.CurrentPage == this._userViewModel;
    public bool ExamButtonActive => this.CurrentPage == this._examViewModel;
    public bool TuitionButtonActive => this.CurrentPage == this._tuitionViewModel;
    public bool AttendanceButtonActive => this.CurrentPage == this._attendanceViewModel;
    public bool ClassButtonActive => this.CurrentPage == this._classViewModel;
    public bool AssignTeacherButtonActive => this.CurrentPage == this._assignTeacherViewModel;
    public bool SubjectClassButtonActive => this.CurrentPage == this._subjectClassViewModel;
    public bool StatisticalButtonActive => this.CurrentPage == this._statisticalViewModel;
    public bool HomeClassButtonActive => this.CurrentPage == this._homeClassViewModel;

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

        // Trang mặc định hiển thị
        _userViewModel = new UserViewModel();
        this.CurrentPage = _userViewModel;
    }

    // - Chuyển trang
    [RelayCommand]
    public void GoToStudentView() => this.CurrentPage = this._studentViewModel;

    // Chuyển đến trang Thông tin giáo viên
    [RelayCommand]
    public void GoToTeacherView() => this.CurrentPage = this._teacherViewModel;

    // Chuyển đến trang Thông tin người dùng
    [RelayCommand]
    public void GoToUserView() => this.CurrentPage = this._userViewModel;

    // Chuyển đến trang Thông tin lịch thi
    [RelayCommand]
    public void GoToExamView() => this.CurrentPage = this._examViewModel;

    // Chuyển đến trang Thông tin học phí
    [RelayCommand]
    public void GoToTuitionView() => this.CurrentPage = this._tuitionViewModel;

    // Chuyển đến trang Thông tin điểm danh
    [RelayCommand]
    public void GoToAttendanceView() => this.CurrentPage = this._attendanceViewModel;

    [RelayCommand]
    public void GoToClassView()=> this.CurrentPage = this._classViewModel;

    [RelayCommand]
    public void GoToAssignTeacherView() => this.CurrentPage = this._assignTeacherViewModel;
    [RelayCommand]
    public void GoToSubjectClassView() => this.CurrentPage = this._subjectClassViewModel;
    [RelayCommand]
    public void GoToStatisticalView() => this.CurrentPage = this._statisticalViewModel;
    [RelayCommand]
    public void GoToHomeClassView() => this.CurrentPage = this._homeClassViewModel;
}
