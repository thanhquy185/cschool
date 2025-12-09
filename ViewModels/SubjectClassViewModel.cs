using System.Collections.ObjectModel;
using System.Reactive;
using Models;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Reactive.Linq;
using ReactiveUI;
using Utils;
using Avalonia;
using Services;

namespace ViewModels;
public partial class SubjectClassViewModel : ViewModelBase
{
    public ObservableCollection<SubjectClassModel> SubjectClasses { get; }
    public ObservableCollection<StudentScoreModel> StudentScores { get; }
    public int TeacherId { get; set;}

    private List<SubjectClassModel> _allSubjectClasses = new List<SubjectClassModel>();
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilter();
            }
        }
    }
    private SubjectClassModel? _selectedSubjectClass;
    public SubjectClassModel? SelectedSubjectClass 
    { 
        get => _selectedSubjectClass;
        set => SetProperty(ref _selectedSubjectClass, value);
    }
    private string _quizCountText = "";
    public string QuizCountText
    {
        get => _quizCountText;
        set => SetProperty(ref _quizCountText, value);
    }

    private string _oralCountText = "";
    public string OralCountText
    {
        get => _oralCountText;
        set => SetProperty(ref _oralCountText, value);
    }

    public ReactiveCommand<int, ObservableCollection<SubjectClassModel>> GetSubjectClassesByTeacherIdCommand { get; }
    public ReactiveCommand<Unit, bool> SaveStudentScoresCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportFromExcelCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportToExcelCommand { get ; }
    public ReactiveCommand<Unit, bool> UpdateScoreColumnsCommand { get; }

public SubjectClassViewModel()
{
    SubjectClasses = new ObservableCollection<SubjectClassModel>();
    StudentScores = new ObservableCollection<StudentScoreModel>();
    TeacherId = 0;
}
    public SubjectClassViewModel(int teacherId)
    {
        SubjectClasses = new ObservableCollection<SubjectClassModel>();
        StudentScores = new ObservableCollection<StudentScoreModel>();

        if(teacherId > 0)
        {
            TeacherId = teacherId;
        }
        else
        {
            TeacherId = 0;
        }
         // Example teacher ID
        Console.WriteLine($"Loading subject classes for Teacher ID: {teacherId}");
        LoadData(TeacherId);


        GetSubjectClassesByTeacherIdCommand = ReactiveCommand.CreateFromTask<int, ObservableCollection<SubjectClassModel>>(async TeacherId =>
        {
            var classes = AppService.SubjectClassService?.GetSubjectClassesByTeacherId(TeacherId) ?? new List<SubjectClassModel>();
            _allSubjectClasses = classes;
            SubjectClasses.Clear();

            foreach (var subjectClass in classes)
            {
                SubjectClasses.Add(subjectClass);
            }
            return SubjectClasses;
        });

        SaveStudentScoresCommand = ReactiveCommand.CreateFromTask<Unit, bool>(async _ =>
        {
            bool success = AppService.SubjectClassService.SaveStudentScores(SelectedSubjectClass, StudentScores.ToList());
            if (success)
            {
                Console.WriteLine("Student scores saved successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to save student scores.");
                return false;
            }
        });

        ImportFromExcelCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(async _ =>
        {
            await ImportFromExcel();
            return Unit.Default;
        }, outputScheduler: RxApp.MainThreadScheduler);

        ExportToExcelCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(async _ =>
        {
            await ExportToExcel();
            return Unit.Default;
        }, outputScheduler: RxApp.MainThreadScheduler);     

        UpdateScoreColumnsCommand = ReactiveCommand.CreateFromTask<Unit, bool>(async _ =>
        {
            if (SelectedSubjectClass == null)
            {
                await MessageBoxUtil.ShowError("Vui lòng chọn lớp môn học để thực hiện thao tác!");
                return false;
            }

            bool success = AppService.SubjectClassService.UpdateScoreColumns(SelectedSubjectClass);
            if (success)
            {
                Console.WriteLine("Score columns updated successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to update score columns.");
                return false;
            }
        });
        
    }

    public void LoadData(int teacherId)
    {
        SubjectClasses.Clear();
        try
        {
            var classes = AppService.SubjectClassService?.GetSubjectClassesByTeacherId(teacherId) ?? new List<SubjectClassModel>();
            _allSubjectClasses = classes;
            foreach (var subjectClass in classes)
            {
                SubjectClasses.Add(subjectClass);
            }
            // If there is a search term currently, apply filter so view shows filtered results
            if (!string.IsNullOrWhiteSpace(SearchText))
                ApplyFilter();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading subject classes: {ex.Message}");
        }
    }

    public void LoadStudentScores (SubjectClassModel subjectClass)
    {
        var students = AppService.SubjectClassService.GetStudentsWithScores(subjectClass);
        try {
            StudentScores.Clear();
            foreach (var student in students)
            {
                StudentScores.Add(student);
                Console.WriteLine($"Added StudentScore: StudentID={student.StudentId}, Name={student.FullName}, OralScores={student.OralScores.Count}, QuizScores={student.Quizzes.Count}, Midterm={student.MidtermScore}, Final={student.FinalScore}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading student scores: {ex.Message}");
        }
      
    }
    public void LoadScoreColumns()
{
    if (SelectedSubjectClass != null)
    {
        QuizCountText = SelectedSubjectClass.QuizCount.ToString();
        OralCountText = SelectedSubjectClass.OralCount.ToString();
    }
}
    public async Task ImportFromExcel()
    {
        if (SelectedSubjectClass == null)
        {
            await MessageBoxUtil.ShowError("Vui lòng chọn lớp môn học để thực hiện thao tác!");
            return;
        }

        // File picker using Avalonia StorageProvider
        var mainWindow = (Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var topLevel = TopLevel.GetTopLevel(mainWindow);
        if (topLevel == null) return;

        var fileOptions = new FilePickerOpenOptions
        {
            Title = "Chọn file Excel để import điểm",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Excel Files") { Patterns = new[] { "*.xlsx", "*.xls" } }
            }
        };

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(fileOptions);
        if (files.Count == 0) return;

        var filePath = files[0].Path.LocalPath;

        try
        {
            var result = AppService.SubjectClassService.ImportStudentScoresFromExcel(SelectedSubjectClass, filePath);
            if (result.success)
            {
                await MessageBoxUtil.ShowSuccess("Nhập điểm từ file Excel thành công!");
                LoadStudentScores(SelectedSubjectClass);
            }
            else
            {
                await MessageBoxUtil.ShowError(result.message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during import: {ex.Message}");
        }
    }

    
    private async Task ExportToExcel()
{
    if (SelectedSubjectClass == null)
    {
        await MessageBoxUtil.ShowError("Vui lòng chọn lớp môn học để thực hiện thao tác!");
        return; 
    }

    // File saver using Avalonia StorageProvider
    var mainWindow = (Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    if (mainWindow == null) return;
    var topLevel = TopLevel.GetTopLevel(mainWindow);
    if (topLevel == null) return;

    var fileOptions = new FilePickerSaveOptions
    {
        Title = "Chọn nơi lưu file Excel xuất điểm",
        SuggestedFileName = $"{SelectedSubjectClass.SubjectName}_{SelectedSubjectClass.ClassName}_Scores.xlsx",
        FileTypeChoices = new List<FilePickerFileType>
        {
            new FilePickerFileType("Excel Files") { Patterns = new[] { "*.xlsx" } }
        }
    };

    var file = await topLevel.StorageProvider.SaveFilePickerAsync(fileOptions);
    if (file == null) return;

    var filePath = file.Path.LocalPath; // Get the local file path

    try
    {
        AppService.SubjectClassService.ExportStudentScoresToExcel(SelectedSubjectClass, StudentScores.ToList(), filePath);
        await MessageBoxUtil.ShowSuccess("Xuất điểm ra file Excel thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during export: {ex.Message}");
    }
}

    private void ApplyFilter()
    {
        var q = (SearchText ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(q))
        {
            SubjectClasses.Clear();
            foreach (var sc in _allSubjectClasses)
                SubjectClasses.Add(sc);
            return;
        }

        var lowered = q.ToLowerInvariant();
        SubjectClasses.Clear();
        foreach (var sc in _allSubjectClasses)
        {
            var subject = sc.SubjectName ?? string.Empty;
            var cls = sc.ClassName ?? string.Empty;
            if (subject.ToLowerInvariant().Contains(lowered) || cls.ToLowerInvariant().Contains(lowered))
            {
                SubjectClasses.Add(sc);
            }
        }
    }
}
