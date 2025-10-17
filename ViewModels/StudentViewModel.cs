using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace cschool.ViewModels
{
    public partial class StudentViewModel : ViewModelBase
    {
        // Danh sách học sinh
        public ObservableCollection<StudentModel> Students { get; }
        public StudentModel? StudentDetails { get; set; }

        // Các command thao tác với học sinh
        public ReactiveCommand<Unit, ObservableCollection<StudentModel>> GetStudentsCommand { get; }
        public ReactiveCommand<int, StudentModel?> GetStudentByIdCommand { get; }
        public ReactiveCommand<StudentModel, bool> CreateStudentCommand { get; }
        public ReactiveCommand<StudentModel, bool> UpdateStudentCommand { get; }
        public ReactiveCommand<StudentModel, bool> LockStudentCommand { get; }

        public StudentViewModel()
        {
            // Load danh sách ban đầu
            Students = new ObservableCollection<StudentModel>();
            var students = AppService.StudentService.GetStudents();
            foreach (var student in students)
                Students.Add(student);

            // Lấy danh sách học sinh
            GetStudentsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var students = AppService.StudentService.GetStudents();
                Students.Clear();
                foreach (var student in students)
                    Students.Add(student);
                return Students;
            });

            // Lấy học sinh theo ID
            GetStudentByIdCommand = ReactiveCommand.Create<int, StudentModel?>(id =>
            {
                var student = AppService.StudentService.GetStudentById(id);
                StudentDetails = student;
                return student;
            });

            // Tạo học sinh mới
            CreateStudentCommand = ReactiveCommand.Create<StudentModel, bool>(student =>
            {
                var result = AppService.StudentService.CreateStudent(student);
                if (result)
                {
                    Students.Add(student);
                }
                return result;
            });

            // // Cập nhật thông tin học sinh
            // UpdateStudentCommand = ReactiveCommand.Create<StudentModel, bool>(student =>
            // {
            //     var result = AppService.StudentService.UpdateStudent(student);
            //     if (result)
            //     {
            //         // Cập nhật lại danh sách
            //         var existing = Students.FirstOrDefault(s => s.Id == student.Id);
            //         if (existing != null)
            //         {
            //             var index = Students.IndexOf(existing);
            //             Students[index] = student;
            //         }
            //     }
            //     return result;
            // });

            // // Khóa hoặc mở khóa học sinh
            // LockStudentCommand = ReactiveCommand.Create<StudentModel, bool>(student =>
            // {
            //     var result = AppService.StudentService.LockStudent(student);
            //     if (result)
            //     {
            //         // Cập nhật trạng thái trong danh sách
            //         var existing = Students.FirstOrDefault(s => s.Id == student.Id);
            //         if (existing != null)
            //         {
            //             existing.IsLocked = student.IsLocked;
            //         }
            //     }
            //     return result;
            // });
        
        }
    }
}
