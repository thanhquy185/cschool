using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using cschool.Models;
using Avalonia.Threading;

using ReactiveUI;

namespace cschool.ViewModels;

public partial class TeacherViewModel : ViewModelBase
{
    public ObservableCollection<TeacherModel> Teachers { get; }
    public ObservableCollection<DepartmentModel> Departments { get; }
    public ReactiveCommand<(int, int?), TeacherModel?> GetTeacherByIdCommand { get; }
    public ReactiveCommand<Unit, ObservableCollection<TeacherModel>> GetTeachersCommand { get; }
    public ReactiveCommand<TeacherModel, bool> CreateTeacherCommand { get; }
    public ReactiveCommand<TeacherModel, bool> UpdateTeacherCommand { get; }
    public ReactiveCommand<int, bool> LockTeacherCommand { get; }

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

    // public List<DepartmentModel> DepartmentOptions
    // {
    //     get
    //     {
    //         var list = new List<DepartmentModel>();
    //         list.Add(new DepartmentModel(0, 0, "---- Chọn Bộ môn ----", "", 1));
    //         list.AddRange(Departments);
    //         return list;
    //     }
    // }
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
                    // Console.WriteLine($"✅ Department changed: {value.Name} (ID={value.Id})");
                }
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
        set => SetProperty(ref _selectedTeacher, value);
    }
    public string SearchText  { get; set; } = "";


    public TeacherViewModel()
    {
        Teachers = new ObservableCollection<TeacherModel>();
        Departments = new ObservableCollection<DepartmentModel>();

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
        GetTeacherByIdCommand = ReactiveCommand.CreateFromTask<(int id, int? termId), TeacherModel?>(async parameters =>
        {
            var (id, termId) = parameters; // giải tuple ra
            return AppService.TeacherService.GetTeacherById(id, termId);
        });


        CreateTeacherCommand = ReactiveCommand.CreateFromTask<TeacherModel, bool>(async (teacher) =>
        {
            try
            {
                string newAvatarFile = await UploadService.SaveImageAsync(teacher.AvatarFile, "teacher", AppService.TeacherService.GetIDLastTeacher() + 1);
                teacher.Avatar = newAvatarFile;
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
                
                var result = AppService.TeacherService?.UpdateTeacher(teacher);
                return result == true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateTeacher Error: {ex.Message}");
                return false;
            }
        });

        // LockTeacherCommand - Khóa giáo viên (set status = 0)
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
    }
    private bool FilterTeachers(object item)
    {
        if (item is not TeacherModel t) return false;

        string search = SearchText?.Trim().ToLower() ?? "";
        string dept = SelectedDepartment?.Name ?? "";

        bool matchSearch = string.IsNullOrEmpty(search)
            || t.Name.ToLower().Contains(search)
            || t.Id.ToString().Contains(search);

        bool matchDepartment = string.IsNullOrEmpty(dept)
            || t.DepartmentName.Equals(dept, StringComparison.OrdinalIgnoreCase);

        return matchSearch && matchDepartment;
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
            var departments = AppService.DepartmentService.GetDepartments();
            foreach (var department in departments)
            {
                Departments.Add(department);            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LoadDepartments Error: {ex.Message}");
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