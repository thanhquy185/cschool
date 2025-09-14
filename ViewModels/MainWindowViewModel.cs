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
    private ViewModelBase _currentPage;
    // - Các biến models
    private readonly StudentViewModel _studentViewModel = new();
    private readonly TeacherViewModel _teacherViewModel = new();

    // Khởi tạo mặc định là trang Thông tin học sinh
    public MainWindowViewModel()
    {
        this.CurrentPage = this._studentViewModel;
    }

    // Các biến giữ trạng thái thể hiện nút được nhấn hoặc không được nhấn
    public bool StudentButtonActive => this.CurrentPage == this._studentViewModel;
    public bool TeacherButtonActive => this.CurrentPage == this._teacherViewModel;

    // Chuyển đến trang Thông tin học sinh
    [RelayCommand]
    public void GoToStudentView() => this.CurrentPage = this._studentViewModel;

    // Chuyển đến trang Thông tin giáo viên
    [RelayCommand]
    public void GoToTeacherView() => this.CurrentPage = this._teacherViewModel;

}
