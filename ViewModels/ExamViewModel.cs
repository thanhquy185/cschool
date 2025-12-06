using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media;
using ReactiveUI;
using cschool.Models;
namespace cschool.ViewModels;

public partial class ExamViewModel : ViewModelBase
{
    // Danh sách lịch thi
    public ObservableCollection<ExamModel> Exams { get; }
    public ObservableCollection<ExamModel> ExamsTemp { get; }
    public ExamModel? ExamDetails { get; set; }
    public ObservableCollection<StudentExamModel> StudentDetails { get; set; }
    public ObservableCollection<TermModel> StudyTerm { get; set; }
    public ObservableCollection<RoomExamModel> RoomDetails { get; set; }
    public ObservableCollection<SubjectModel> SubjectList { get; set; }
    public ObservableCollection<RoomModel> RoomList { get; set; }
    public ObservableCollection<RoomModel> RoomListUpdate { get; set; }
    public ObservableCollection<TeacherModel> TeacherList { get; set; }
    public ObservableCollection<TeacherModel> TeacherListUpdate { get; set; }
    public ObservableCollection<ExamAssignment> ExamAssignments { get; } = new();
    public ObservableCollection<RoomModel> SelectedRooms { get; } = new();
    public ObservableCollection<TeacherModel> SelectedTeachers { get; } = new();
    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public string FilterKeyword { get; set; } = "";
    public string FilterStatus { get; set; } = "";
    public int TotalStudents { get; set; }
    private int _remainingStudents;
    public int RemainingStudents
    {
        get => _remainingStudents;
        set
        {
            if (_remainingStudents != value)
            {
                _remainingStudents = value;
                OnPropertyChanged(nameof(RemainingStudents));
                RemainingStudentsText = $"Còn lại {_remainingStudents} học sinh chưa được phân vào phòng thi";
            }
        }
    }
    private int _remainingUpdateStudents;
    public int RemainingUpdateStudents
    {
        get => _remainingUpdateStudents;
        set
        {
            if (_remainingUpdateStudents != value)
            {
                _remainingUpdateStudents = value;
                OnPropertyChanged(nameof(RemainingUpdateStudents));
                RemainingStudentsText = $"Còn lại {_remainingUpdateStudents} học sinh chưa được phân vào phòng thi";
            }
        }
    }
    private string _remainingStudentsText = "";
    public string RemainingStudentsText
    {
        get => _remainingStudentsText;
        set
        {
            if (_remainingStudentsText != value)
            {
                _remainingStudentsText = value;
                OnPropertyChanged(nameof(RemainingStudentsText));
            }
        }
    }
    private int? _selectedGrade;
    public int? SelectedGrade
    {
        get => _selectedGrade;
        set
        {
            if (_selectedGrade != value)
            {
                _selectedGrade = value;
                OnPropertyChanged(nameof(SelectedGrade));
                RecalculateRemainingStudents();
            }
        }
    }
    private int? _selectedUpdateTerm;
    public int? SelectedUpdateTerm
    {
        get => _selectedUpdateTerm;
        set
        {
            if (_selectedUpdateTerm != value)
            {
                _selectedUpdateTerm = value;
                OnPropertyChanged(nameof(SelectedUpdateTerm));
                RecalculateRemainingStudents();
            }
        }
    }
    public void RecalculateRemainingStudents()
    {
        if (SelectedGrade is null || SelectedUpdateTerm is null)
        {
            RemainingStudents = 0;
            return;
        }

        // Tổng số học sinh theo khối/lớp
        int total = AppService.ExamService.GetStudentGrade(SelectedGrade.Value,SelectedUpdateTerm.Value);

        // Tổng số học sinh đã phân công trong ExamAssignments
        int assigned = ExamAssignments.Sum(a => a.AssignedStudents);

        int remaining = Math.Max(total - assigned, 0);

        // Gán RemainingStudents và cập nhật text hiển thị luôn
        RemainingStudents = remaining;

        // Nếu bạn dùng property string riêng cho binding TextBlock
        RemainingStudentsText = $"Còn lại {remaining} học sinh chưa được phân vào phòng thi";
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Các command thao tác với lịch thi
    public ReactiveCommand<Unit, ObservableCollection<ExamModel>> GetExamsCommand { get; }
    public ReactiveCommand<int, Unit> GetExamByIdCommand { get; }
    public ReactiveCommand<(int details_id, int room_id), Unit> GetStudentExamByIdCommand { get; }
    public ReactiveCommand<int, Unit> GetRoomExamByIdCommand { get; }
    public ReactiveCommand<int, Unit> GetRoomUpdateByIdCommand { get; }
    public ReactiveCommand<int, Unit> GetRoomListUpdateCommand { get; }
    public ReactiveCommand<int, Unit> GetTeacherListUpdateCommand { get; }
    public ReactiveCommand<ExamCreateModel, bool> CreateExamCommand { get; }
    public ReactiveCommand<ExamUpdateModel, bool> UpdateExamCommand { get; }
    public ReactiveCommand<ExamModel, bool> LockExamCommand { get; }

    public List<string> GradeList { get; set; } = new List<string>
    {
        "10", "11", "12"
    };
    public TermModel? SelectedTerm { get; set; }
    public SubjectModel? SelectedSubject { get; set; }

    // Hàm lọc dữ liệu dựa trên Exams
    public void ApplyFilter()
    {
        if (Exams == null || Exams.Count == 0)
            return;

        var filtered = Exams.Where(s =>
        {
            bool matchKeyword =
                string.IsNullOrWhiteSpace(FilterKeyword) ||
                s.Subject.Contains(FilterKeyword, StringComparison.OrdinalIgnoreCase) ||
                s.Id.ToString().Contains(FilterKeyword);

            bool matchStatus =
                string.IsNullOrWhiteSpace(FilterStatus) ||
                FilterStatus == "Chọn học kỳ" ||
                $"{s.TermName} - {s.TermYear}".Equals(FilterStatus, StringComparison.OrdinalIgnoreCase);

            return matchKeyword && matchStatus;
        }).ToList();

        // Cập nhật ExamsTemp hiển thị
        ExamsTemp.Clear();
        foreach (var s in filtered)
            ExamsTemp.Add(s);
    }

    public ExamViewModel()
    {
        // Load danh sách ban đầu
        Exams = new ObservableCollection<ExamModel>();
        ExamsTemp = new ObservableCollection<ExamModel>();
        StudentDetails = new ObservableCollection<StudentExamModel>();
        RoomDetails = new ObservableCollection<RoomExamModel>();
        
        var exams = AppService.ExamService.GetExamSchedule();
        foreach (var exam in exams)
        {
            Exams.Add(exam);
            ExamsTemp.Add(exam);
        }

        // Lấy danh sách lịch thi
        GetExamsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var exams = AppService.ExamService.GetExamSchedule();
            Exams.Clear();
            foreach (var exam in exams)
                Exams.Add(exam);

            ExamsTemp.Clear();
            foreach (var e in Exams)
                ExamsTemp.Add(e);
            return ExamsTemp;
        });

        // Lấy lịch thi theo ID
        GetExamByIdCommand = ReactiveCommand.Create<int>(id =>
        {
            var exam = AppService.ExamService.GetExamById(id);
            ExamDetails = exam;
            // Tìm Term theo tên trong list
            SelectedTerm = StudyTerm.FirstOrDefault(t => t.TermName == $"{exam.TermName} - {exam.TermYear}");
            // Tìm Subject theo tên
            SelectedSubject = SubjectList.FirstOrDefault(s => s.SubjectName == exam.Subject);
        });

        // Lấy danh sách phòng thi theo lịch
        GetRoomExamByIdCommand = ReactiveCommand.CreateFromTask<int>(async (id) =>
        {
            var room_exam = AppService.ExamService.GetRoomExamById(id);
            RoomDetails.Clear();
            foreach (var s in room_exam)
                RoomDetails.Add(s);
        });

        // Lấy danh sách học sinh theo phòng
        GetStudentExamByIdCommand = ReactiveCommand.CreateFromTask<(int details_id, int room_id), Unit>(
        async tuple =>
        {
            var (details_id, room_id) = tuple;

            // Gọi hàm đồng bộ bên service trong Task.Run để không chặn UI
            var student_exam = await Task.Run(() =>
                AppService.ExamService.GetStudentExamById(details_id, room_id)
            );

            StudentDetails.Clear();
            foreach (var s in student_exam)
                StudentDetails.Add(s);

            return Unit.Default;
        });

        // Lấy danh sách môn học
        SubjectList = new ObservableCollection<SubjectModel>();
        var subjects = AppService.ExamService.GetSubjectList();
        foreach (var subject in subjects)
            SubjectList.Add(subject);

        // Lấy danh sách học kỳ
        StudyTerm = new ObservableCollection<TermModel>();
        var terms = AppService.ExamService.GetTermList();
        foreach (var term in terms)
            StudyTerm.Add(term);
      
        // Lấy danh sách phòng thi
        RoomList = new ObservableCollection<RoomModel>();
        var rooms = AppService.ExamService.GetRoomList();
        foreach (var room in rooms)
            RoomList.Add(room);

        // Lấy danh sách giáo viên
        TeacherList = new ObservableCollection<TeacherModel>();
        var teachers = AppService.ExamService.GetTeacherList();
        foreach (var teacher in teachers)
            TeacherList.Add(teacher);

        // Tạo lịch thi mới
        CreateExamCommand = ReactiveCommand.CreateFromTask<ExamCreateModel, bool>(async (exam) =>
        {
            var result = AppService.ExamService.CreateExam(exam);
            if (result)
            {
                return true;
            }
            return false;
        });

        // Lấy danh sách phân công của cập nhật
        ExamAssignments = new ObservableCollection<ExamAssignment>();
        GetRoomUpdateByIdCommand = ReactiveCommand.CreateFromTask<int>(async (id) =>
        {
            var rooms_update = AppService.ExamService.GetRoomById(id);
            ExamAssignments.Clear();
            foreach (var s in rooms_update)
                ExamAssignments.Add(s);
        });

        // Lấy danh sách phòng thi của cập nhật
        RoomListUpdate = new ObservableCollection<RoomModel>();
        GetRoomListUpdateCommand = ReactiveCommand.CreateFromTask<int>(async (id) =>
        {
            var roomsUpdate = AppService.ExamService.GetRoomListUpdate(id);
            RoomListUpdate.Clear();
            foreach (var roomUpdate in roomsUpdate)
                RoomListUpdate.Add(roomUpdate);
        });

        // Lấy danh sách giáo viên của cập nhật
        TeacherListUpdate = new ObservableCollection<TeacherModel>();
        GetTeacherListUpdateCommand = ReactiveCommand.CreateFromTask<int>(async (id) =>
        {
            var teachersUpdate = AppService.ExamService.GetTeacherListUpdate(id);
            TeacherListUpdate.Clear();
            foreach (var teacherUpdate in teachersUpdate)
                TeacherListUpdate.Add(teacherUpdate);
        });


        // Cập nhật thông tin lịch thi
        UpdateExamCommand = ReactiveCommand.CreateFromTask<ExamUpdateModel, bool>(async (exam) =>
        {
            var result = AppService.ExamService.UpdateExam(exam);
            if (result)
            {
                return true;
            }
            return false;
        });

        // Khóa lịch thi
        LockExamCommand = ReactiveCommand.Create<ExamModel, bool>(exam =>
        {
            var result = AppService.ExamService.DeleteExam(exam.Id);
            if (result)
            {
                return true;
            }
            return false;
        });
    
    }

}