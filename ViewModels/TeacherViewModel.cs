using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Models;
using ClosedXML.Excel;
using ReactiveUI;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Utils;
using System.Reactive.Threading.Tasks;
using Avalonia;
using Avalonia.Platform.Storage;



namespace ViewModels;

public partial class TeacherViewModel : ViewModelBase
{
    public ObservableCollection<TeacherModel> Teachers { get; }
    public ObservableCollection<DepartmentModel> Departments { get; }
    public ObservableCollection<UserModel> Users { get; }
    public ReactiveCommand<Unit, TeacherModel?> GetTeacherByIdCommand { get; }
    public ReactiveCommand<Unit, ObservableCollection<TeacherModel>> GetTeachersCommand { get; }
    public ReactiveCommand<TeacherModel, bool> CreateTeacherCommand { get; }
    public ReactiveCommand<TeacherModel, bool> UpdateTeacherCommand { get; }
    public ReactiveCommand<int, bool> LockTeacherCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportFromExcelCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportToExcelCommand { get; }
    

    public List<KeyValuePair<int, string>> StatusOptions { get; } = new()
    {
        new KeyValuePair<int, string>(1, "Hoạt động"),
        new KeyValuePair<int, string>(0, "Ẩn")
    };
    public List<string> GenderOptions { get; } = new()
    {
        "--- Chọn Giới tính ---",
        "Nam",
        "Nữ"
    };
    public List<DepartmentModel> DepartmentOptions
    {
        get
        {
            var list = new List<DepartmentModel>();
            list.Add(new DepartmentModel(0, 0, "---- Chọn Bộ môn ----", "", 1));
            list.AddRange(Departments);
            return list;
        }
    }
    
    private TeacherModel? _teacherDetails;
    public TeacherModel? TeacherDetails
    {
        get => _teacherDetails;
        set => SetProperty(ref _teacherDetails, value);
    }
    private DepartmentModel? _selectedDepartment;
    public DepartmentModel? SelectedDepartment
    {
        get => _selectedDepartment;
        set
        {
            if (SetProperty(ref _selectedDepartment, value))
            {
                // Auto-update TeacherDetails khi chọn department
                if (value != null && TeacherDetails != null && value.Id != 0)
                {
                    TeacherDetails.DepartmentId = value.Id;
                    TeacherDetails.DepartmentName = value.Name;
                }
            }
        }
    }

    private DepartmentModel? _selectedDepartmentFilter;
    public DepartmentModel? SelectedDepartmentFilter
    {
        get => _selectedDepartmentFilter;
        set
        {
            if (SetProperty(ref _selectedDepartmentFilter, value))
            {
                ApplyFilter();
            }
        }
    }

    private KeyValuePair<int, string>? _selectedStatus;
    public KeyValuePair<int, string>? SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            if (SetProperty(ref _selectedStatus, value))
            {
                if (value.HasValue && TeacherDetails != null)
                {
                    TeacherDetails.Status = value.Value.Key;
                }
            }
        }
    }
    private string? _selectedGender;
    public string? SelectedGender
    {
        get => _selectedGender;
        set
        {
            if (SetProperty(ref _selectedGender, value))
            {
                if (value != null && TeacherDetails != null)
                {
                    TeacherDetails.Gender = value;
                }
            }
        }
    }
    private TeacherModel? _selectedTeacher;
    public TeacherModel? SelectedTeacher
    {
        get => _selectedTeacher;
        set { 
            SetProperty(ref _selectedTeacher, value);
            Console.WriteLine($"SelectedTeacher changed: {value?.Id}");
        }

    }
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


    public TeacherViewModel()
    {
        Teachers = new ObservableCollection<TeacherModel>();
        Departments = new ObservableCollection<DepartmentModel>();
        Users = new ObservableCollection<UserModel>();

        LoadData();
        // lấy ds giáo viên
        GetTeachersCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var teachers = AppService.TeacherService.GetTeachers();
            
                Teachers.Clear();
                foreach (var teacher in teachers)
                {
                    Teachers.Add(teacher);
                }
            
            return Teachers;
        });

        // Lấy giáo viên theo ID
        GetTeacherByIdCommand = ReactiveCommand.CreateFromTask<Unit, TeacherModel?>(async _ =>
        {
            var teacher = AppService.TeacherService.GetTeacherById(SelectedTeacher.Id);
            return teacher;
        });


        CreateTeacherCommand = ReactiveCommand.CreateFromTask<TeacherModel, bool>(async (teacher) =>
        {
            try
            {
                string newAvatarFile = await UploadService.SaveImageAsync(teacher.AvatarFile, "teacher", AppService.TeacherService.GetIDLastTeacher() + 1);
                teacher.Avatar = newAvatarFile;
                var user = new UserModel{
                    Username = teacher.Username,
                    Password = teacher.Password,
                    Fullname = teacher.Name,
                    RoleId = 2, // role_id = 2 cho giáo viên
                    Status = "Hoạt động",
                    Avatar = newAvatarFile,
                    Phone = teacher.Phone,
                    Email = teacher.Email,
                    Address = teacher.Address
                };
                var userResult = AppService.UserService.CreateUser(user);
                int userId = AppService.UserService.GetIdLastUser();
                teacher.UserId = userId;
                var result = AppService.TeacherService.CreateTeacher(teacher);
                if (result) return true;
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateTeacher Error: {ex.Message}");
                return false;
            }
        });

        UpdateTeacherCommand = ReactiveCommand.CreateFromTask<TeacherModel, bool>(async (teacher) =>
        {
            try
            {
                if (teacher == null)
                {
                    Console.WriteLine("UpdateTeacher Error: teacher is null");
                    return false;
                }

                if (!string.IsNullOrEmpty(teacher.AvatarFile) && File.Exists(teacher.AvatarFile))
                {
                    string updatedAvatarFile = await UploadService.SaveImageAsync(teacher.AvatarFile, "teacher", teacher.Id);
                    teacher.Avatar = updatedAvatarFile;
                }
                var user = new UserModel{
                    Id = SelectedTeacher.UserId,
                    RoleId = 2,
                    Fullname = teacher.Name,
                    Avatar = teacher.Avatar ?? "",
                    Phone = teacher.Phone,
                    Email = teacher.Email,
                    Address = teacher.Address,
                };
                var result = AppService.UserService.UpdateUser(user);
                if (result <= 0) return false;
                var resultTeacher = AppService.TeacherService?.UpdateTeacher(teacher);
                return resultTeacher == true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateTeacher Error: {ex.Message}");
                return false;
            }
        });

        LockTeacherCommand = ReactiveCommand.CreateFromTask<int, bool>(async (teacherId) =>
        {
            try
            {                
                return await Task.Run(() =>
                {
                    var result = AppService.TeacherService.LockTeacher(teacherId);
                    return result;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LockTeacher Error: {ex.Message}");
                return false;
            }
        });

        ImportFromExcelCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(async param =>
        {
            await ImportFromExcel();
            return Unit.Default;
        }, outputScheduler: RxApp.MainThreadScheduler);

        ExportToExcelCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(async _ =>
        {
            await ExportToExcel();
            return Unit.Default;
        }, outputScheduler: RxApp.MainThreadScheduler);   
    }

    public void LoadData()
    {

        Teachers.Clear();
        try
        {
            var teachers = AppService.TeacherService.GetTeachers();
            foreach (var teacher in teachers)
            {
                Teachers.Add(teacher);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LoadTeachers Error: {ex.Message}");
        }


        Departments.Clear();
        try
        {
            var departments = AppService.TeacherService.GetDepartments();
            foreach (var department in departments)
            {
                Departments.Add(department);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LoadDepartments Error: {ex.Message}");
        }

        Users.Clear();
        try
        {
            var users = AppService.UserService.GetUsers();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LoadUsers Error: {ex.Message}");
        }
    }
    
    private async Task ImportFromExcel()
    {
        // File picker using Avalonia StorageProvider
        var mainWindow = (Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow == null) return;
        var topLevel = TopLevel.GetTopLevel(mainWindow);
        if (topLevel == null) return;

        var fileOptions = new FilePickerOpenOptions
        {
            Title = "Chọn file Excel để import giáo viên",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Excel Files") { Patterns = new[] { "*.xlsx", "*.xls" } }
            }
        };

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(fileOptions);
        if (files.Count == 0) return;

        var filePath = files[0].Path.LocalPath; // Get the local file path

        try
        {
            bool success = AppService.TeacherService.ImportTeachersFromExcel(filePath);
            if (success)
            {
                await MessageBoxUtil.ShowSuccess("Nhập giáo viên từ file Excel thành công!");
                // Reload data after import
                LoadData();
            }
            else
            {
                await MessageBoxUtil.ShowError("Nhập giáo viên từ file Excel thất bại!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during import: {ex.Message}");
        }
    }

    private async Task ExportToExcel()
    {
        if (Teachers.Count == 0)
        {
            await MessageBoxUtil.ShowError("Không có giáo viên để xuất!");
            return; // Or show a message dialog
        }

        // File saver using Avalonia StorageProvider
        var mainWindow = (Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow == null) return;
        var topLevel = TopLevel.GetTopLevel(mainWindow);
        if (topLevel == null) return;

        var fileOptions = new FilePickerSaveOptions
        {
            Title = "Chọn nơi lưu file Excel xuất danh sách giáo viên",
            SuggestedFileName = "Teachers.xlsx",
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
            AppService.TeacherService.ExportTeachersToExcel(Teachers.ToList(), filePath);
            Console.WriteLine("Exported teachers successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during export: {ex.Message}");
        }
    }
    private void ApplyFilter()
    {
        var departmentName = SelectedDepartmentFilter?.Name ?? "";

        var allTeachers = AppService.TeacherService.GetTeachers();

        var filteredTeachers = allTeachers.Where(t =>
        {
            bool matchesSearch = string.IsNullOrEmpty(SearchText) ||
                                t.Id.ToString().Contains(SearchText) ||
                                t.Name.ToLower().Contains(SearchText);

            bool matchesDepartment = string.IsNullOrEmpty(departmentName) ||
                                    departmentName == "---- Chọn Bộ môn ----" ||
                                    t.DepartmentName.Equals(departmentName, StringComparison.OrdinalIgnoreCase);

            return matchesSearch && matchesDepartment;
        }).ToList();

        // Update UI
        Teachers.Clear();
        foreach (var teacher in filteredTeachers)
        {
            Teachers.Add(teacher);
        }
    }


    public void SetTeacherForEdit(TeacherModel teacher)
    {
        TeacherDetails = teacher;
        SelectedStatus = StatusOptions.FirstOrDefault(x => x.Key == teacher.Status);
        SelectedGender = teacher.Gender;
        SelectedDepartment = Departments.FirstOrDefault(d => d.Id == teacher.DepartmentId);
    }
    public void ClearTeacherDetails()
    {
        TeacherDetails = null;
        SelectedStatus = null;
        SelectedGender = null;
        SelectedDepartment = null;
    }
}

