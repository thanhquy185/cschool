
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace cschool.ViewModels;

public partial class ExamViewModel : ViewModelBase
{
    // Danh sách lịch thi
    public ObservableCollection<ExamModel> Exams { get; }
    public ExamModel? ExamDetails { get; set; }
    public string FilterKeyword { get; set; } = "";
    public string FilterStatus { get; set; } = "";

    // Các command thao tác với lịch thi
    public ReactiveCommand<Unit, ObservableCollection<ExamModel>> GetExamsCommand { get; }
    public ReactiveCommand<int, ExamModel?> GetExamByIdCommand { get; }
    public ReactiveCommand<ExamModel, bool> CreateExamCommand { get; }
    public ReactiveCommand<ExamModel, bool> UpdateExamCommand { get; }
    public ReactiveCommand<ExamModel, bool> LockExamCommand { get; }

    public ExamViewModel()
    {
        // Load danh sách ban đầu
        Exams = new ObservableCollection<ExamModel>();

        var exams = AppService.ExamService.GetExamSchedule();
        foreach (var exam in exams)
        {
            Exams.Add(exam);
        }

        // Lấy danh sách lịch thi
        GetExamsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var exams = AppService.ExamService.GetExamSchedule();
            Exams.Clear();
            foreach (var exam in exams)
                Exams.Add(exam);
            return Exams;
        });

        // Lấy lịch thi theo ID
        // GetExamByIdCommand = ReactiveCommand.Create<int, ExamModel?>(id =>
        // {
        //     var Exam = AppService.ExamService.GetExamById(id);
        //     ExamDetails = Exam;
        //     return Exam;
        // });

        // // Tạo lịch thi mới
        // CreateStudentCommand = ReactiveCommand.CreateFromTask<ExamModel, bool>(async (student) =>
        // {
        //     string newAvatarFile = await UploadService.SaveImageAsync(student.AvatarFile, "student", AppService.StudentService.GetIdLastStudent() + 1);
        //     student.Avatar = newAvatarFile;
        //     var result = AppService.StudentService.CreateStudent(student);
        //     if (result)
        //     {
        //         return true;
        //     }
        //     return false;
        // });

        // // Cập nhật thông tin lịch thi
        // UpdateStudentCommand = ReactiveCommand.CreateFromTask<ExamModel, bool>(async (student) =>
        // {
        //     // Nếu người dùng có chọn ảnh mới (AvatarFile không rỗng)
        //     if (!string.IsNullOrEmpty(student.AvatarFile) && File.Exists(student.AvatarFile))
        //     {
        //         string? newAvatarFile = await UploadService.SaveImageAsync(student.AvatarFile, "student", student.Id);
        //         student.Avatar = newAvatarFile;
        //     }
        //     var result = AppService.StudentService.UpdateStudent(student);
        //     if (result)
        //     {
        //         return true;
        //     }
        //     return false;
        // });

        // // Khóa lịch thi
        // LockStudentCommand = ReactiveCommand.Create<ExamModel, bool>(student =>
        // {
        //     var result = AppService.StudentService.LockStudent(student);
        //     if (result)
        //     {
        //         return true;
        //     }
        //     return false;
        // });
    
    }

}