using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace cschool.ViewModels
{
    public partial class StudentViewModel : ViewModelBase
    {
        // Danh sách học sinh
        public ObservableCollection<StudentModel> Students { get; }
        // Dữ liệu hiển thị (lọc / tìm kiếm)
        public ObservableCollection<StudentModel> StudentsTemp { get; }
        public ObservableCollection<StudentModel> AllStudents { get; }
        public StudentModel? StudentDetails { get; set; }
        public string FilterKeyword { get; set; } = "";
        public string FilterStatus { get; set; } = "";

        // Các command thao tác với học sinh
        public ReactiveCommand<Unit, ObservableCollection<StudentModel>> GetStudentsCommand { get; }
        public ReactiveCommand<int, StudentModel?> GetStudentByIdCommand { get; }
        public ReactiveCommand<StudentModel, bool> CreateStudentCommand { get; }
        public ReactiveCommand<StudentModel, bool> UpdateStudentCommand { get; }
        public ReactiveCommand<StudentModel, bool> LockStudentCommand { get; }
        public List<string> LearnStatusOptions { get; } = new()
        {
            "--- Chọn Tình trạng học ---",
            "Đang học",
            "Tốt nghiệp",
            "Bảo lưu",
            "Nghỉ học"
        };
        public List<string> GenderOptions { get; } = new()
        {
            "--- Chọn Giới tính ---",
            "Nam",
            "Nữ"
        };

        // Hàm lọc dữ liệu dựa trên Students
        public void ApplyFilter()
        {
            if (Students == null || Students.Count == 0)
                return;

            var filtered = Students.Where(s =>
            {
                bool matchKeyword =
                    string.IsNullOrWhiteSpace(FilterKeyword) ||
                    s.Fullname.Contains(FilterKeyword, StringComparison.OrdinalIgnoreCase) ||
                    s.Id.ToString().Contains(FilterKeyword);

                bool matchStatus =
                    string.IsNullOrWhiteSpace(FilterStatus) ||
                    FilterStatus == "--- Chọn Tình trạng học ---" ||
                    s.LearnStatus.Equals(FilterStatus, StringComparison.OrdinalIgnoreCase);

                return matchKeyword && matchStatus;
            }).ToList();

            // Cập nhật StudentsTemp hiển thị
            StudentsTemp.Clear();
            foreach (var s in filtered)
                StudentsTemp.Add(s);
        }
        public StudentViewModel()
        {
            // Load danh sách ban đầu
            Students = new ObservableCollection<StudentModel>();
            StudentsTemp = new ObservableCollection<StudentModel>();

            var students = AppService.StudentService.GetStudents();
            foreach (var student in students)
            {
                Students.Add(student);
                StudentsTemp.Add(student);
            }

            AllStudents = new ObservableCollection<StudentModel>();
            var allstudents = AppService.StudentService.GetAllStudents();
            foreach (var student in allstudents)
                AllStudents.Add(student);

            // Lấy danh sách học sinh
            GetStudentsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var students = AppService.StudentService.GetStudents();
                Students.Clear();
                foreach (var student in students)
                    Students.Add(student);

                // Reset lại danh sách hiển thị
                StudentsTemp.Clear();
                foreach (var s in Students)
                    StudentsTemp.Add(s);

                return StudentsTemp;
            });

            // Lấy học sinh theo ID
            GetStudentByIdCommand = ReactiveCommand.Create<int, StudentModel?>(id =>
            {
                var student = AppService.StudentService.GetStudentById(id);
                StudentDetails = student;
                return student;
            });

            // Tạo học sinh mới
            CreateStudentCommand = ReactiveCommand.CreateFromTask<StudentModel, bool>(async (student) =>
            {
                string newAvatarFile = await UploadService.SaveImageAsync(student.AvatarFile, "student", AppService.StudentService.GetIdLastStudent() + 1);
                student.Avatar = newAvatarFile;
                var result = AppService.StudentService.CreateStudent(student);
                if (result)
                {
                    return true;
                }
                return false;
            });

            // Cập nhật thông tin học sinh
            UpdateStudentCommand = ReactiveCommand.CreateFromTask<StudentModel, bool>(async (student) =>
            {
                // Nếu người dùng có chọn ảnh mới (AvatarFile không rỗng)
                if (!string.IsNullOrEmpty(student.AvatarFile) && File.Exists(student.AvatarFile))
                {
                    string? newAvatarFile = await UploadService.SaveImageAsync(student.AvatarFile, "student", student.Id);
                    student.Avatar = newAvatarFile;
                }
                var result = AppService.StudentService.UpdateStudent(student);
                if (result)
                {
                    return true;
                }
                return false;
            });

            // Khóa học sinh
            LockStudentCommand = ReactiveCommand.Create<StudentModel, bool>(student =>
            {
                var result = AppService.StudentService.LockStudent(student);
                if (result)
                {
                    return true;
                }
                return false;
            });
        }
    }
}
