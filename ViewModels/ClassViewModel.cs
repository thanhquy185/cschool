using Models;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using Avalonia.Threading;

namespace ViewModels;

public class ClassViewModel : ViewModelBase
{
    public ObservableCollection<ClassModel> Classes { get; } = new();
    public ObservableCollection<ClassModel> Classes_list { get; } = new();

    public ObservableCollection<TeacherModel> Teachers { get; } = new();

    // Filter list
    public ObservableCollection<StudentModel> FilteredStudentHK1 { get; } = new();
    public ObservableCollection<StudentModel> FilteredStudentHK2 { get; } = new();

    public ObservableCollection<int> Grades { get; } = new() { 10, 11, 12 };
    public int SelectedGrade { get; set; }

    public ObservableCollection<ClassTypeModel> ClassTypes { get; } = new();
    public ReactiveCommand<Unit, bool> CreateClassCommand { get; }

    public string SelectedClassType { get; set; }
    private TeacherModel _selectedTeacherHK1;
    public TeacherModel SelectedTeacherHK1
    {
        get => _selectedTeacherHK1;
        set
        {
            if (_selectedTeacherHK1 != value)
            {
                _selectedTeacherHK1 = value;
                OnPropertyChanged(nameof(SelectedTeacherHK1));
            }
        }
    }
        private TeacherModel _selectedTeacherHK2;
    public TeacherModel SelectedTeacherHK2
    {
        get => _selectedTeacherHK2;
        set
        {
            if (_selectedTeacherHK2 != value)
            {
                _selectedTeacherHK2 = value;
                OnPropertyChanged(nameof(SelectedTeacherHK2));
            }
        }
    }

    // HK1
    public ObservableCollection<StudentModel> StudentsAvailableHK1 { get; } = new();
    public ObservableCollection<StudentModel> StudentInClassHK1 { get; } = new();
    public IList<object> SelectedStudentsHK1 { get; set; } = new List<object>();

    // HK2
    public ObservableCollection<StudentModel> StudentsAvailableHK2 { get; } = new();
    public ObservableCollection<StudentModel> StudentInClassHK2 { get; } = new();
    public IList<object> SelectedStudentsHK2 { get; set; } = new List<object>();
    private ClassTypeModel _selectedClassTypeModel;
    public ClassTypeModel SelectedClassTypeModel
    {
        get => _selectedClassTypeModel;
        set => SetProperty(ref _selectedClassTypeModel, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
    private string _searchClassText = "";
    public string SearchClassText
    {
        get => _searchClassText;
        set
        {
            if (SetProperty(ref _searchClassText, value))
                ApplyClassFilter();
        }
    }

    private string _selectedYear = "Chọn năm học";
    public string SelectedYear
    {
        get => _selectedYear;
        set
        {
            if (SetProperty(ref _selectedYear, value))
                ApplyClassFilter();
        }
    }

// List các năm học có trong data
public ObservableCollection<string> YearList { get; } = new ObservableCollection<string>();



    public ReactiveCommand<Unit, Unit> AddStudentsToClassHK1Command { get; }
    public ReactiveCommand<Unit, Unit> RemoveStudentsFromClassHK1Command { get; }
    public ReactiveCommand<Unit, Unit> AddStudentsToClassHK2Command { get; }
    public ReactiveCommand<Unit, Unit> RemoveStudentsFromClassHK2Command { get; }
    public ReactiveCommand<Unit, bool> UpdateClassCommand { get; }
    public ReactiveCommand<int, bool> DeleteClassCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetFilterCommand { get; }



    private string _year;
    public string Year
    {
        get => _year;
        set => SetProperty(ref _year, value);
    }

    private string _searchStudentTextHK1 = "";
    public string SearchStudentTextHK1
    {
        get => _searchStudentTextHK1;
        set
        {
            if (SetProperty(ref _searchStudentTextHK1, value))
                FilterStudents(StudentsAvailableHK1, value, FilteredStudentHK1);
        }
    }

    private string _searchStudentTextHK2 = "";
    public string SearchStudentTextHK2
    {
        get => _searchStudentTextHK2;
        set
        {
            if (SetProperty(ref _searchStudentTextHK2, value))
                FilterStudents(StudentsAvailableHK2, value, FilteredStudentHK2);
        }
    }

    private TeacherModel _teacherHK1 = new();
    public TeacherModel TeacherHK1
    {
        get => _teacherHK1;
        set => SetProperty(ref _teacherHK1, value);
    }

    private TeacherModel _teacherHK2 = new();
    public TeacherModel TeacherHK2
    {
        get => _teacherHK2;
        set => SetProperty(ref _teacherHK2, value);
    }

    public ReactiveCommand<int, Unit> GetClassByIdCommand { get; }
    public ReactiveCommand<int, Unit> GetTeacherByIdCommand { get; }

    public void LoadStudentsWithoutClass()
    {
        if (string.IsNullOrWhiteSpace(Year) || SelectedGrade == 0)
            return;

        StudentsAvailableHK1.Clear();
        StudentsAvailableHK2.Clear();
        FilteredStudentHK1.Clear();
        FilteredStudentHK2.Clear();

        var list = AppService.ClassService.GetUnassignedStudents(Year, SelectedGrade);

        foreach (var s in list)
        {
            StudentsAvailableHK1.Add(s);
            StudentsAvailableHK2.Add(s);
            FilteredStudentHK1.Add(s);
            FilteredStudentHK2.Add(s);
        }
    }

    private ClassModel _selectedClass;
    public ClassModel SelectedClass
    {
        get => _selectedClass;
        set => SetProperty(ref _selectedClass, value);
    }

    public ReactiveCommand<Unit, bool> ConfirmCommand { get; }


    public void LoadData()
    {  
       
         SelectedClass = new ClassModel();
        Classes.Clear();
        Classes_list.Clear();
        foreach (var c in AppService.ClassService.GetClasses())
            Classes.Add(c);
            YearList.Clear();
YearList.Add("Chọn năm học");
foreach (var year in Classes.Select(c => c.Year).Distinct())
    YearList.Add(year);

// Hiển thị tất cả lớp khi chưa filter
ApplyClassFilter();

    Classes_list.Clear();
        foreach (var c in Classes
            .Where(x => x.Status != 0)
            .GroupBy(x => x.Id)
            .Select(g => g.First()))
        {
            Classes_list.Add(c);
        }



        Teachers.Clear();
        foreach (var t in AppService.TeacherService.GetTeachers())
            Teachers.Add(t);

        ClassTypes.Clear();
        foreach (var ct in AppService.ClassService.GetClasstype())
            ClassTypes.Add(ct);
    }

    public ClassViewModel()
    {

        LoadData();
        // LoadClassData();

    
        
        // Load students of class
        GetClassByIdCommand = ReactiveCommand.Create<int>(id =>
        {
            StudentInClassHK1.Clear();
            StudentInClassHK2.Clear();

            foreach (var s in AppService.ClassService.GetStudentsByClassId(id, 1))
                StudentInClassHK1.Add(s);

            foreach (var s in AppService.ClassService.GetStudentsByClassId(id, 2))
                StudentInClassHK2.Add(s);

            FilteredStudentHK1.Clear();
            foreach (var s in StudentInClassHK1) FilteredStudentHK1.Add(s);

            FilteredStudentHK2.Clear();
            foreach (var s in StudentInClassHK2) FilteredStudentHK2.Add(s);
        });

        GetTeacherByIdCommand = ReactiveCommand.Create<int>(classId =>
        {
            var termMap = AppService.ClassService.GetTermMapByClass(classId);

            if (termMap.TryGetValue("Học kỳ 1", out int t1))
                TeacherHK1 = AppService.ClassService.GetTeacherByClassAndTerm(classId, t1);

            if (termMap.TryGetValue("Học kỳ 2", out int t2))
                TeacherHK2 = AppService.ClassService.GetTeacherByClassAndTerm(classId, t2);
        });

                ResetFilterCommand = ReactiveCommand.Create(() =>
        {
            SearchClassText = "";
            SelectedYear = "Chọn năm học";
        });

  

        AddStudentsToClassHK1Command =
            ReactiveCommand.Create(
                () => MoveStudents(SelectedStudentsHK1, StudentsAvailableHK1, StudentInClassHK1),
                outputScheduler: RxApp.MainThreadScheduler);

        RemoveStudentsFromClassHK1Command =
            ReactiveCommand.Create(
                () => MoveStudents(SelectedStudentsHK1, StudentInClassHK1, StudentsAvailableHK1),
                outputScheduler: RxApp.MainThreadScheduler);

        AddStudentsToClassHK2Command =
            ReactiveCommand.Create(
                () => MoveStudents(SelectedStudentsHK2, StudentsAvailableHK2, StudentInClassHK2),
                outputScheduler: RxApp.MainThreadScheduler);

        RemoveStudentsFromClassHK2Command =
            ReactiveCommand.Create(
                () => MoveStudents(SelectedStudentsHK2, StudentInClassHK2, StudentsAvailableHK2),
                outputScheduler: RxApp.MainThreadScheduler);

        ConfirmCommand = ReactiveCommand.CreateFromTask<bool>(async () =>
    {
        if (SelectedClass == null)
            SelectedClass = new ClassModel();

        SelectedClass.Grade = SelectedGrade;

        if (SelectedClassTypeModel != null)
        {
            SelectedClass.ClassTypeId = SelectedClassTypeModel.Id;
            SelectedClass.ClassTypeName = SelectedClassTypeModel.Name;
        }
        else
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
                Console.WriteLine("Chọn loại lớp trước!"));
            return false;
        }

        SelectedClass.TeacherHK1 = SelectedTeacherHK1;
        SelectedClass.TeacherHK2 = SelectedTeacherHK2;

        if (string.IsNullOrWhiteSpace(SelectedClass.Name))
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
                Console.WriteLine("Tên lớp không được để trống!"));
            return false;
        }

        if (SelectedGrade == 0)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
                Console.WriteLine("Chọn khối lớp trước!"));
            return false;
        }

        if (string.IsNullOrWhiteSpace(Year))
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
                Console.WriteLine("Năm học không được để trống!"));
            return false;
        }

        try
        {
            // Lưu lớp học (không ảnh hưởng UI, chạy background được)
            int classId = await AppService.ClassService.SaveClassAsync(SelectedClass, Year);

            // Lưu học sinh (không ảnh hưởng UI)
            if (StudentInClassHK1.Count > 0)
                await AppService.ClassService.AssignStudentsToClassAsync(classId, 1, Year, StudentInClassHK1.ToList());

            if (StudentInClassHK2.Count > 0)
                await AppService.ClassService.AssignStudentsToClassAsync(classId, 2, Year, StudentInClassHK2.ToList());

            // Thông báo UI
            await Dispatcher.UIThread.InvokeAsync(() =>
                Console.WriteLine("Lưu lớp học thành công!"));

            return true;
        }
        catch (Exception ex)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
                Console.WriteLine("Lỗi khi lưu lớp: " + ex.Message));
            return false;
        }
    }, outputScheduler: RxApp.MainThreadScheduler);


    }

    // MOVE CHUNG
private void MoveStudents(
    IList<object> selected,
    ObservableCollection<StudentModel> from,
    ObservableCollection<StudentModel> to)
{
    if (selected == null || selected.Count == 0) return;

    foreach (StudentModel s in selected.Cast<StudentModel>())
    {
        if (!to.Contains(s))
            to.Add(s);

        from.Remove(s);
    }
}


    private void FilterStudents(
        ObservableCollection<StudentModel> source,
        string text,
        ObservableCollection<StudentModel> target)
    {
        target.Clear();
        if (string.IsNullOrWhiteSpace(text))
        {
            foreach (var s in source) target.Add(s);
            return;
        }

        string lower = text.ToLower();
        foreach (var s in source.Where(x =>
                   x.Fullname?.ToLower().Contains(lower) == true ||
                   x.Id.ToString().Contains(lower)))
            target.Add(s);
    }
    public void ResetState()
    {
        SelectedClass = null;
        SelectedClassTypeModel = null;

        SelectedTeacherHK1 = null;
        SelectedTeacherHK2 = null;

        StudentsAvailableHK1.Clear();
        StudentInClassHK1.Clear();

        StudentsAvailableHK2.Clear();
        StudentInClassHK2.Clear();

        SearchStudentTextHK1 = string.Empty;
        SearchStudentTextHK2 = string.Empty;
    }


    public void ValidateSchoolYear()
    {
        if (string.IsNullOrWhiteSpace(Year))
            return;

        if (Year.Length == 4 && int.TryParse(Year, out int startYear))
            Year = $"{startYear}-{startYear + 1}";

        if (SelectedGrade != 0)
            LoadStudentsWithoutClass();
    }

  public void LoadClassData()
{
    // Nếu chưa chọn class thực sự thì không làm gì
    if (SelectedClass == null || SelectedClass.Id == 0) 
        return;

    // Lấy 2 row theo học kỳ (an toàn)
    var clsHK1 = Classes.FirstOrDefault(c => c.Id == SelectedClass.Id && c.Term == "Học kỳ 1");
    var clsHK2 = Classes.FirstOrDefault(c => c.Id == SelectedClass.Id && c.Term == "Học kỳ 2");

    // Gán chung từ HK1 (nếu có)
    if (clsHK1 != null)
    {
        SelectedGrade = clsHK1.Grade;
        Year = clsHK1.Year;
        SelectedClassTypeModel = ClassTypes.FirstOrDefault(ct => ct.Id == clsHK1.ClassTypeId);

        System.Console.WriteLine("GVCN1: " + clsHK1.HeadTeacher);
    }

    // Gán SelectedTeacherHK1 an toàn (nếu có)
    if (clsHK1 != null && !string.IsNullOrWhiteSpace(clsHK1.HeadTeacher))
    {
        if (int.TryParse(clsHK1.HeadTeacher, out var t1Id))
            SelectedTeacherHK1 = Teachers.FirstOrDefault(t => t.Id == t1Id);
        else
            SelectedTeacherHK1 = Teachers.FirstOrDefault(t => t.Name == clsHK1.HeadTeacher); // fallback theo tên
    }
    else
    {
        SelectedTeacherHK1 = null;
    }

    // Gán SelectedTeacherHK2 an toàn (nếu có)
    if (clsHK2 != null && !string.IsNullOrWhiteSpace(clsHK2.HeadTeacher))
    {
        if (int.TryParse(clsHK2.HeadTeacher, out var t2Id))
            SelectedTeacherHK2 = Teachers.FirstOrDefault(t => t.Id == t2Id);
        else
            SelectedTeacherHK2 = Teachers.FirstOrDefault(t => t.Name == clsHK2.HeadTeacher); // fallback theo tên
    }
    else
    {
        SelectedTeacherHK2 = null;
    }

    // Load danh sách học sinh (chỉ khi SelectedClass.Id hợp lệ)
    StudentInClassHK1.Clear();
    foreach (var s in AppService.ClassService.GetStudentsByClassId(SelectedClass.Id, 1))
        StudentInClassHK1.Add(s);

    StudentInClassHK2.Clear();
    foreach (var s in AppService.ClassService.GetStudentsByClassId(SelectedClass.Id, 2))
        StudentInClassHK2.Add(s);

    // Load danh sách học sinh chưa có lớp
    LoadStudentsWithoutClass();

    // Loại bỏ những học sinh đã có trong lớp khỏi danh sách StudentsAvailable
    foreach (var s in StudentInClassHK1.ToList())
        StudentsAvailableHK1.Remove(s);

    foreach (var s in StudentInClassHK2.ToList())
        StudentsAvailableHK2.Remove(s);
    }

            public (bool success, string message) DeleteClassById(int id)
    {
        return AppService.ClassService.DeleteClass(id);
    }

    
private void ApplyClassFilter()
{
    Classes_list.Clear();

    var filtered = Classes.AsEnumerable();

    // Lọc theo tên lớp
    if (!string.IsNullOrWhiteSpace(SearchClassText))
        filtered = filtered.Where(c => c.Name?.ToLower().Contains(SearchClassText.ToLower()) == true);

    // Lọc theo năm học
    if (!string.IsNullOrWhiteSpace(SelectedYear) && SelectedYear != "Chọn năm học")
        filtered = filtered.Where(c => c.Year == SelectedYear);

    foreach (var c in filtered)
        Classes_list.Add(c);
}





}
