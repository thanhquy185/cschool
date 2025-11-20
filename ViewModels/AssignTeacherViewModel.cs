using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using cschool.Models;
using cschool.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Threading;
using System.Threading.Tasks;
using Avalonia;
using cschool.Views.DialogAssignTeacher;
using Avalonia.Controls.ApplicationLifetimes;
using cschool.Utils;
using ClassModel = cschool.Models.Classes;
namespace cschool.ViewModels;

public partial class AssignTeacherViewModel : ViewModelBase
{
    private readonly AssignTeacherService _service;

    public ObservableCollection<AssignTeacher> AssignTeachers { get; } = new();
    public ObservableCollection<TeacherModel> Teachers { get; } = new();
    public ObservableCollection<Subjects> Subjects { get; } = new();
    public ObservableCollection<ClassModel> Classes { get; } = new();
    public ObservableCollection<string> DaysOfWeek { get; } = new();
    

    [ObservableProperty]
    private AssignTeacher? _selectedAssignTeacher;

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
    private int _quizCount;

    [ObservableProperty]
    private int _oralCount;

    [ObservableProperty]
    private int _start;

    [ObservableProperty]
    private int _end;

    private AssignTeacher? _editingItem;
    
   
    // SỬA: Sử dụng RelayCommand của CommunityToolkit
    [RelayCommand]
    public void LoadData()
    {
        try
        {
            var assignTeachers = _service.GetAssignTeachers() ?? new List<AssignTeacher>();
            var teachers = _service.GetTeachers() ?? new List<TeacherModel>();
            var subjects = _service.GetCourses() ?? new List<Subjects>();
            var classes = _service.GetClasses() ?? new List<ClassModel>();
            var days = _service.GetDaysOfWeek(DateTime.Now) ?? new List<string>();

                AssignTeachers.Clear();
                Teachers.Clear();
                Subjects.Clear();
                Classes.Clear();
                DaysOfWeek.Clear();

                foreach (var a in assignTeachers)
                    AssignTeachers.Add(a);

                foreach (var t in teachers)
                    Teachers.Add(t);

                foreach (var s in subjects)
                    Subjects.Add(s);

                foreach (var c in classes)
                    Classes.Add(c);

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
    [RelayCommand]
    public async Task SaveAdd()

    {
        var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

        if (SelectedTeacher == null || SelectedSubject == null || SelectedClass == null || string.IsNullOrEmpty(SelectedDay))
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn đầy đủ dữ liệu", owner: owner);
            return;
        }
            try
            {
            var assign = new AssignTeacher(
                SelectedClass.Assign_class_Id,
                SelectedTeacher.Id,
                SelectedSubject.Id,
                SelectedSubject.Name,
                SelectedClass.Name,
                SelectedTeacher.Name,
                SelectedClass.Room,
                SelectedDay,
                Start,
                End
            )
            {
                QuizCount = 2,
                OralCount = 2
            };
            if (_service.IsTeacherBusy(assign.Teachers_id, assign.Day, assign.Start, assign.End))
            {
                await MessageBoxUtil.ShowError("Giáo viên đã có lịch dạy vào khung giờ này!", owner: owner);
                return;
            }

                if (_service.AddAssignmentTeacher(assign))
                {
                    await MessageBoxUtil.ShowSuccess("Thêm phân công thành công", owner: owner);
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
    
[RelayCommand]
public async Task SaveEdit()

    {
    var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
         
            if (SelectedTeacher == null || SelectedSubject == null || SelectedClass == null || string.IsNullOrEmpty(SelectedDay))
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn đầy đủ dữ liệu", owner: owner);
                return;
            }
        if (_service.IsConflict(_editingItem))
        {
            await MessageBoxUtil.ShowError("Giáo viên đã có lịch dạy vào khung giờ này!", owner: owner);
            return;
        }
            try
            {
           
                _editingItem.Teachers_id = SelectedTeacher.Id;
                _editingItem.Subject_id = SelectedSubject.Id;
                _editingItem.Assign_class_id = SelectedClass.Assign_class_Id;
                _editingItem.ClassName = SelectedClass.Name;
                _editingItem.Day = SelectedDay;
                _editingItem.Start = Start;
                _editingItem.End = End;
                _editingItem.QuizCount = QuizCount;
                 _editingItem.OralCount = OralCount;

            // Gọi update
            
                if (_service.Update(_editingItem))
                {
                    await MessageBoxUtil.ShowSuccess("Cập nhật thành công",owner: owner);
                    LoadDataCommand.Execute(null);
                    (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                    .MainWindow?.OwnedWindows
                    .OfType<AssignTeacherAddDialog>()
                    .FirstOrDefault()?
                    .Close(true);
                }
                else
                {
                    await MessageBoxUtil.ShowError("Cập nhật thất bại",owner:owner);
                    Console.WriteLine("Error: Could not update assignment.");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error updating assignment: {ex.Message}");
            }
        }



    [RelayCommand]
    private async Task OpenEditDialog(AssignTeacher a)
    {
        var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        LoadDataCommand.Execute(null);
        // ⚙️ Dừng một chút để UI thread cập nhật (nếu cần)
        await Task.Delay(100);

    _editingItem = a;
    SelectedTeacher = Teachers.FirstOrDefault(t => t.Id == a.Teachers_id);
    SelectedSubject = Subjects.FirstOrDefault(s => s.Id == a.Subject_id);
    SelectedClass = Classes.FirstOrDefault(c => c.Assign_class_Id == a.Assign_class_id);
    SelectedDay = a.Day;
    Start = a.Start;
    End = a.End;
    QuizCount = a.QuizCount;
    OralCount = a.OralCount;

    var dialog = new AssignTeacherEditDialog
    {
        DataContext = this
    };
    await dialog.ShowDialog(
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow
    );
}
[RelayCommand]
private async Task OpenDetailDialog(AssignTeacher a)
    {
        var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        LoadDataCommand.Execute(null);
        // ⚙️ Dừng một chút để UI thread cập nhật (nếu cần)
        await Task.Delay(100);

    _editingItem = a;

    SelectedTeacher = Teachers.FirstOrDefault(t => t.Id == a.Teachers_id);
    SelectedSubject = Subjects.FirstOrDefault(s => s.Id == a.Subject_id);
    SelectedClass = Classes.FirstOrDefault(c => c.Assign_class_Id == a.Assign_class_id);
    SelectedDay = a.Day;
    Start = a.Start;
    End = a.End;
    QuizCount = a.QuizCount;
    OralCount = a.OralCount;

    var dialog = new AssignTeacherDetailDialog
    {
        DataContext = this
    };
    await dialog.ShowDialog(
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow
    );
}
    [RelayCommand]
    public async Task Delete(AssignTeacher a)
    {
        var owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (a == null)
        {
            await MessageBoxUtil.ShowError("vui lòng chọn 1 dòng để xóa", owner: owner);
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
            results = _service.GetAssignTeachers() ?? new List<AssignTeacher>();
        }
        else
        {
            results = _service.Search(keyword);
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
        var results = _service.Search(SelectedSubjectSearch.Name ?? "");
        Dispatcher.UIThread.Post(() =>
        {
            AssignTeachers.Clear();
            foreach (var a in results)
                AssignTeachers.Add(a);
        });
    }

  [RelayCommand]
private async Task OpenAddDialog()
{
    try
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _editingItem = null;
            SelectedTeacher = null;
            SelectedSubject = null;
            SelectedClass = null;
            SelectedDay = null;
            Start = 0;
            End = 0;
            // QuizCount = 0;
            // OralCount = 0;
        });
        
        // ⏳ Đợi data load xong
        // await Task.Delay(200);
        
        var dialog = new AssignTeacherAddDialog
        {
            DataContext = this
        };
        
        var result = await dialog.ShowDialog<bool>(
            (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow
        );
        
        if (result)
        {
            LoadDataCommand.Execute(null);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error opening add dialog: {ex.Message}");
    }
}

[RelayCommand]
public void ResetSearch()
{
    SearchText = string.Empty;
    LoadDataCommand.Execute(null); // 🔁 Hiển thị lại toàn bộ danh sách
}

    public AssignTeacherViewModel(AssignTeacherService service)
    {
        _service = service;
        LoadDataCommand.Execute(null);
    }
    
}