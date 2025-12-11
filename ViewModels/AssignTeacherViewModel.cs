using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models;
using Services;
using Avalonia.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Utils;
using ClassModel = Models.Classes;
using System.ComponentModel;
namespace ViewModels;

public partial class AssignTeacherViewModel : ViewModelBase
{
    // Tiêu đề trang
    public string TitlePage { get; } = "Quản lý phân công giáo viên";
    // Mô tả trang
    public string DescriptionPage { get; } = "Quản lý phân công dạy học cho giáo viên";
    private readonly AssignTeacherService _service;

    public ObservableCollection<AssignTeacher> AssignTeachers { get; } = new();
    public ObservableCollection<TeacherModel> Teachers { get; } = new();
    public ObservableCollection<TeacherModel> Teachers1 { get; } = new();
    public ObservableCollection<Subjects> Subjects { get; } = new();
    public ObservableCollection<ClassModel> Classes { get; } = new();
    public ObservableCollection<string> DaysOfWeek { get; } = new();
    public ObservableCollection<TermModel> Terms { get; set; } = new();

    [ObservableProperty]
    private string _loadingStatus = "Chọn môn học để hiển thị giáo viên";

    [ObservableProperty]
    private AssignTeacher? _selectedAssignTeacher;
      [ObservableProperty]
    private TermModel? _selectedTerm;

    [ObservableProperty]
    private TeacherModel? _selectedTeacher;

    [ObservableProperty]
    private Subjects? _selectedSubject;
    [ObservableProperty]
    private Subjects? _selectedSubjectSearch;

    [ObservableProperty]
    private ClassModel? _selectedClass;

    [ObservableProperty]
    private string? _selectedDay;

    [ObservableProperty]
    private bool _isFormVisible;

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private string _quizCount;

    [ObservableProperty]
    private string _oralCount;

    [ObservableProperty]
    private string _start;

    [ObservableProperty]
    private string _end;

    private AssignTeacher? _editingItem;

    // SỬA: Thêm property để theo dõi việc đang tải giáo viên
    [ObservableProperty]
    private bool _isLoadingTeachers = false;

    // SỬA: Xử lý khi chọn môn học thay đổi
    partial void OnSelectedSubjectChanged(Subjects? value)
    {
        if (value != null)
        {
            LoadTeachersBySubject(value.Id);
        }
        else
        {
            // Nếu không chọn môn học, clear danh sách giáo viên
            Teachers.Clear();
            LoadingStatus = "Chọn môn học để hiển thị giáo viên";
        }
    }
    private async void LoadTeachersBySubject(int subjectId)
    {
        try
        {
            IsLoadingTeachers = true;
            LoadingStatus = "Đang tải danh sách giáo viên...";

            // Clear danh sách giáo viên hiện tại
            Teachers.Clear();

            // Lấy danh sách giáo viên theo môn học
            var teachersBySubject = await Task.Run(() => _service.GetTeachers(subjectId));

            // Thêm giáo viên vào danh sách
            foreach (var teacher in teachersBySubject)
            {
                Teachers.Add(teacher);
            }

            LoadingStatus = teachersBySubject.Count > 0
                ? $"Đã tải {teachersBySubject.Count} giáo viên"
                : "Không có giáo viên nào cho môn học này";


        }
        catch (Exception ex)
        {
            LoadingStatus = "Lỗi khi tải danh sách giáo viên";
            Console.WriteLine($"❌ Lỗi khi tải giáo viên theo môn học: {ex.Message}");
        }
        finally
        {
            IsLoadingTeachers = false;
        }
    }

    [RelayCommand]
    public void LoadTerms()
    {
        try
        {
            var terms = _service.GetTerms() ?? new List<TermModel>();
            Terms.Clear();
            foreach (var t in terms)
                Terms.Add(t);

            // Mặc định chọn học kỳ hiện tại (nếu có)
             SelectedTerm = Terms.LastOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading terms: {ex.Message}");
        }
    }
    // SỬA: Sử dụng RelayCommand của CommunityToolkit
    [RelayCommand]
    public void LoadData()
    {
        try
        {

            var assignTeachers = _service.GetAssignTeachers(SelectedTerm.Id) ?? new List<AssignTeacher>();
            var subjects = _service.GetCourses() ?? new List<Subjects>();
            var teacher1 = _service.GetTeachers() ?? new BindingList<TeacherModel>();
            var classes = _service.GetClasses(SelectedTerm.Id) ?? new List<ClassModel>();
            var days = _service.GetDaysOfWeek(DateTime.Now) ?? new List<string>();

            AssignTeachers.Clear();
            Subjects.Clear();
            Classes.Clear();
            DaysOfWeek.Clear();
            Teachers.Clear();
            Teachers1.Clear();
            
            foreach (var a in assignTeachers)
                AssignTeachers.Add(a);

            foreach (var s in subjects)
                Subjects.Add(s);

            foreach (var c in classes)
                Classes.Add(c);

            foreach (var t in teacher1)
                Teachers1.Add(t);

            foreach (var d in days)
                DaysOfWeek.Add(d);

            Console.WriteLine("📘 Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }
    // Hàm tìm kiếm khi nhập dữ liệu
    partial void OnSearchTextChanged(string value)
    {
        Search();
    }

    public event EventHandler? RequestCloseAddDialog;
    [RelayCommand]
    public async Task SaveAdd()

    {
        var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

        if (SelectedTeacher == null || SelectedSubject == null || SelectedClass == null || string.IsNullOrEmpty(SelectedDay) || Start == "" || End == "")
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn đầy đủ dữ liệu", owner: owner);
            return;
        }
        if (Rules.IsNumeric(Start) || Rules.IsNumeric(End))
        {
            await MessageBoxUtil.ShowError("vui lòng nhập dữ liệu số", owner: owner);
            return;
        }
        if (int.Parse(Start) <= 0 || int.Parse(End) <= 0)
        {
            await MessageBoxUtil.ShowError("Tiết bắt đầu và kết thúc phải là số dương", owner: owner);
            return;
        }
        if (int.Parse(End) > 10)
        {
            await MessageBoxUtil.ShowError("Tiết kết thúc tối thiểu là 10", owner: owner);
            return;
        }

        if (int.Parse(Start) >= int.Parse(End))
        {
            await MessageBoxUtil.ShowError("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc", owner: owner);
            return;
        }

        try
        {
            var assign = new AssignTeacher
            {
                Assign_class_id = SelectedClass.Assign_class_Id,
                Teachers_id = SelectedTeacher.Id,
                Subject_id = SelectedSubject.Id,
                CourseName = SelectedSubject.Name_Subject,
                ClassName = SelectedClass.Name,
                Teachers = SelectedTeacher.Name,
                RoomName = SelectedClass.Room,
                Day = SelectedDay,
                Start = int.Parse(Start),
                End = int.Parse(End),
                QuizCount = 2,
                OralCount = 2
            };

            if (_service.IsTeacherBusy(assign.Teachers_id, assign.Day, assign.Start, assign.End, assign.Assign_class_id))
            {
                await MessageBoxUtil.ShowError("Giáo viên đã có lịch dạy vào khung giờ này!", owner: owner);
                return;
            }
            if (_service.IsClassBusy(assign.Assign_class_id, assign.Day, assign.Start, assign.End))
            {
                await MessageBoxUtil.ShowError("Lớp học đã có lịch học vào khung giờ này!", owner: owner);
                return;
            }

            if (_service.AddAssignmentTeacher(assign))
            {
                await MessageBoxUtil.ShowSuccess("Thêm phân công thành công", owner: owner);
                RequestCloseAddDialog?.Invoke(this, EventArgs.Empty);
                // (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                // .MainWindow?.OwnedWindows
                // .OfType<AssignTeacherAddDialog>()
                // .FirstOrDefault()?
                // .Close(true);
                LoadDataCommand.Execute(null);

            }
            else
            {
                await MessageBoxUtil.ShowError("Thêm phân công thất bại", owner: owner);
                Console.WriteLine("Error: Could not add assignment.");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding assignment: {ex.Message}");
        }
    }

    public event EventHandler? RequestCloseEditDialog;
    [RelayCommand]
    public async Task SaveEdit()
    {
        var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

        if (SelectedTeacher == null || SelectedSubject == null || SelectedClass == null || string.IsNullOrEmpty(SelectedDay) || Start == "" || End == "" || OralCount == "" || QuizCount == "")
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn đầy đủ dữ liệu", owner: owner);
            return;
        }
        if (Rules.IsNumeric(Start) || Rules.IsNumeric(End))
        {
            await MessageBoxUtil.ShowError("Tiết bắt đầu và kết thúc phải là số", owner: owner);
            return;
        }
        if (Rules.IsNumeric(OralCount) || Rules.IsNumeric(QuizCount))
        {
            await MessageBoxUtil.ShowError("Số bài kiểm tra phải là số", owner: owner);
            return;
        }
        if (Convert.ToInt32(OralCount) <= 0 || Convert.ToInt32(QuizCount) <= 0)
        {
            await MessageBoxUtil.ShowError("Số bài kiểm tra phải là số dương", owner: owner);
            return;
        }
        if (Convert.ToInt32(Start) <= 0 || Convert.ToInt32(End) <= 0)
        {
            await MessageBoxUtil.ShowError("Tiết bắt đầu và tiết kết thúc phải là số dương", owner: owner);
            return;
        }
        if (int.Parse(End) > 10)
        {
            await MessageBoxUtil.ShowError("Tiết kết thúc tối thiểu là 10", owner: owner);
            return;
        }

        if (Convert.ToInt32(Start) >= Convert.ToInt32(End))
        {
            await MessageBoxUtil.ShowError("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc", owner: owner);
            return;
        }

        if (_service.IsConflict(_editingItem))
        {
            await MessageBoxUtil.ShowError("Giáo viên đã có lịch dạy vào khung giờ này!", owner: owner);
            return;
        }
        try
        {
            _editingItem.Subject_id = SelectedSubject.Id;
            _editingItem.Teachers_id = SelectedTeacher.Id;
            _editingItem.Assign_class_id = SelectedClass.Assign_class_Id;
            _editingItem.ClassName = SelectedClass.Name;
            _editingItem.Day = SelectedDay;
            _editingItem.Start = Convert.ToInt32(Start);
            _editingItem.End = Convert.ToInt32(End);
            _editingItem.QuizCount = Convert.ToInt32(QuizCount);
            _editingItem.OralCount = Convert.ToInt32(OralCount);

            // Gọi update
            if (_service.Update(_editingItem))
            {
                await MessageBoxUtil.ShowSuccess("Cập nhật thành công", owner: owner);
                LoadDataCommand.Execute(null);
                // (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                // .MainWindow?.OwnedWindows
                // .OfType<AssignTeacherEditDialog>()
                // .FirstOrDefault()?
                // .Close(true);
                RequestCloseEditDialog?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                await MessageBoxUtil.ShowError("Cập nhật thất bại ", owner: owner);
                Console.WriteLine("Error: Could not update assignment.");
                Console.WriteLine($"Giáo viên đc chọn để sửa: ID={SelectedTeacher.Id}, Name={SelectedTeacher.Name}, Address={SelectedTeacher.Address}, Department={SelectedTeacher.DepartmentName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating assignment: {ex.Message}");
        }
    }

    public event EventHandler<AssignTeacher>? RequestOpenEditDialog;

    [RelayCommand]
    private async Task OpenEditDialog(AssignTeacher a)
    {
        // LoadDataCommand.Execute(null);
        await Task.Delay(100);

        _editingItem = a;

        SelectedSubject = Subjects.FirstOrDefault(s => s.Id == a.Subject_id);

        // Teacher ưu tiên trong Teachers
        SelectedTeacher = Teachers.FirstOrDefault(t => t.Id == a.Teachers_id);

        // Nếu không có → fallback từ Teachers1
        if (SelectedTeacher == null)
        {
            var fallbackTeacher = Teachers1.FirstOrDefault(t => t.Id == a.Teachers_id);
            if (fallbackTeacher != null)
            {
                Teachers.Add(fallbackTeacher);
                SelectedTeacher = fallbackTeacher;
            }
        }

        SelectedClass = Classes.FirstOrDefault(c => c.Assign_class_Id == a.Assign_class_id);

        SelectedDay = a.Day;
        Start = a.Start.ToString();
        End = a.End.ToString();
        QuizCount = a.QuizCount.ToString();
        OralCount = a.OralCount.ToString();

        // 👉 Bắn tín hiệu cho View mở dialog
        RequestOpenEditDialog?.Invoke(this, a);
    }

    public event EventHandler<AssignTeacher>? RequestOpenDetailDialog;
    [RelayCommand]
    private async Task OpenDetailDialog(AssignTeacher a)
    {
        var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        // LoadDataCommand.Execute(null);
        // ⚙️ Dừng một chút để UI thread cập nhật (nếu cần)
        await Task.Delay(100);

        _editingItem = a;

        SelectedTeacher = Teachers1.FirstOrDefault(t => t.Id == a.Teachers_id);
        SelectedSubject = Subjects.FirstOrDefault(s => s.Id == a.Subject_id);
        SelectedClass = Classes.FirstOrDefault(c => c.Assign_class_Id == a.Assign_class_id);
        SelectedDay = a.Day;
        Start = a.Start.ToString();
        End = a.End.ToString();
        QuizCount = a.QuizCount.ToString();
        OralCount = a.OralCount.ToString();

        RequestOpenDetailDialog?.Invoke(this, a);
    }
    [RelayCommand]
    public async Task Delete(AssignTeacher a)
    {
        var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (a == null)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn 1 dòng để xóa", owner: owner);
            return;
        }
        if (await MessageBoxUtil.ShowConfirm("Bạn có chắc chắn muốn xóa phân công này không?"))
        {


            if (_service.DeleteAssignTeacher(a))
            {
                await MessageBoxUtil.ShowSuccess("Xóa thành công", owner: owner);
                LoadDataCommand.Execute(null);
            }
            else
            {
                await MessageBoxUtil.ShowError("Xóa thất bại", owner: owner);
            }
        }
    }

    [RelayCommand]
    public void Search()
    {
        try
        {
            // _editingItem = a;
            // SelectedTeacher = TeacherModels.FirstOrDefault(t => t.Id == a.Teachers_id);
            // SelectedSubject = Subjects.FirstOrDefault(s => s.Id == a.Subject_id);
            // SelectedClass = Classes.FirstOrDefault(c => c.Assign_class_Id == a.Assign_class_id);
            // SelectedDay = a.Day;
            // Start = a.Start;
            // End = a.End;
            // QuizCount = a.QuizCount;
            // OralCount = a.OralCount;
            // IsFormVisible = true;

            var keyword = _searchText?.Trim() ?? "";
            IEnumerable<AssignTeacher> results;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Nếu trống → load lại toàn bộ
                results = _service.GetAssignTeachers(SelectedTerm.Id) ?? new List<AssignTeacher>();
            }
            else
            {
                results = _service.Search(SelectedTerm.Id,keyword);
            }

            Dispatcher.UIThread.Post(() =>
            {
                AssignTeachers.Clear();
                foreach (var a in results)
                    AssignTeachers.Add(a);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Search error: {ex.Message}");
        }
    }
    [RelayCommand]
    public void SearchNameSubject()
    {
        var results = _service.Search(SelectedTerm.Id,SelectedSubjectSearch.Name_Subject ?? "");
        Dispatcher.UIThread.Post(() =>
        {
            AssignTeachers.Clear();
            foreach (var a in results)
                AssignTeachers.Add(a);
        });
    }
    partial void OnSelectedTermChanged(TermModel? value)
    {
        if (value != null)
        {
            Console.WriteLine($"Đã chọn học kỳ: {value.Name} - Năm: {value.Year}");
            LoadDataCommand.Execute(null);
        }
    }

    // [RelayCommand]
    // private async Task OpenAddDialog()
    // {
    //     try
    //     {
    //         var dialog = new AssignTeacherAddDialog
    //         {
    //             DataContext = this
    //         };

    //         var result = await dialog.ShowDialog<bool>(
    //             (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow
    //         );

    //         if (result)
    //         {
    //             LoadDataCommand.Execute(null);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"❌ Error opening add dialog: {ex.Message}");
    //     }
    // }

    [RelayCommand]
    public void ResetSearch()
    {
        SearchText = string.Empty;
        LoadDataCommand.Execute(null); // 🔁 Hiển thị lại toàn bộ danh sách
    }
    public void ClearForm()
    {
        _editingItem = null;
        SelectedTeacher = null;
        SelectedSubject = null;
        SelectedClass = null;
        SelectedDay = null;
        Start = "";
        End = "";
    }



    [ObservableProperty]
    public bool _infoButtonEnabled;
    [ObservableProperty]
    public bool _createButtonEnabled;
    [ObservableProperty]
    public bool _updateButtonEnabled;
    [ObservableProperty]
    public bool _lockButtonEnabled;

    public AssignTeacherViewModel()
    {
        // Phân quyền các nút chức năng
        if (SessionService.currentUserLogin != null && AppService.RoleDetailService != null)
        {
            this.InfoButtonEnabled = AppService.RoleDetailService.HasPermission(
                        SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Class, "Xem");
            this.CreateButtonEnabled = AppService.RoleDetailService.HasPermission(
               SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Class, "Thêm");
            this.UpdateButtonEnabled = AppService.RoleDetailService.HasPermission(
               SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Class, "Cập nhật");
            this.LockButtonEnabled = AppService.RoleDetailService.HasPermission(
               SessionService.currentUserLogin.RoleId, (int)FunctionIdEnum.Class, "Xoá / Khoá");

        }

        this._service = AppService.AssignTeacherService;
     

        LoadTermsCommand.Execute(null);
    }

}