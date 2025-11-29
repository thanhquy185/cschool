using System;
using System.Collections.Generic;
using System.ComponentModel;

public class ExamModel
{
    public int Id { get; set; }
    public string Room { get; set; }
    public string Subject { get; set; }
    public string TeacherName { get; set; }
    public string ExamDate { get; set; }
    public string TermName { get; set; }
    public string TermYear { get; set; }
    public string CandidateCount { get; set; }
    public string Grade { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public int SubjectId { get; set; }
    public int TermId { get; set; }
    public string ExamDateDisplay
    {
        get
        {
            if (DateTime.TryParse(ExamDate, out var date))
                return date.ToString("dd/MM/yyyy");
            return ExamDate;
        }
    }
    public string StartTimeDisplay
    {
        get
        {
            if (DateTime.TryParse(StartTime, out var date))
                return date.ToString("HH:mm");
            return StartTime;
        }
    }
    public string EndTimeDisplay
    {
        get
        {
            if (DateTime.TryParse(EndTime, out var date))
                return date.ToString("HH:mm");
            return EndTime;
        }
    }

}

public class StudentExamModel
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public string ClassName { get; set; }
}

public class RoomExamModel
{
    public int Id { get; set; }
    public string RoomName { get; set; }
    public string TeacherName { get; set; }
    public string RoomQuantity { get; set; }
}

public class SubjectModel
{
    public int Id { get; set; }
    public string SubjectName { get; set; }
}

public class RoomModel
{
    public int Id { get; set; }
    public string RoomName { get; set; }
    public int Quantity { get; set; }
}

public class TeacherModel
{
    public int Id { get; set; }
    public string TeacherName { get; set; }
}

public class TermModel
{
    public int Id { get; set; }
    public string TermName { get; set; }
}

public class ExamAssignment : INotifyPropertyChanged
{
    public string RoomName { get; set; }
    public string TeacherName { get; set; }
    public int RoomQuantity { get; set; }
    private int _assignedStudents;
    public int AssignedStudents
    {
        get => _assignedStudents;
        set
        {
            if (_assignedStudents != value)
            {
                _assignedStudents = value;
                OnPropertyChanged(nameof(AssignedStudents));
            }
        }
    }
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
            }
        }
    }
    public int MaxAssignable => Math.Min(RoomQuantity, RemainingStudents);
    public RoomModel? Room { get; set; }
    public TeacherModel? Teacher { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class ExamCreateModel
{
    public int SubjectId { get; set; }
    public int TermId { get; set; }
    public int GradeId { get; set; }
    public string ExamDate { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public List<ExamAssignmentCreateModel> Assignments { get; set; } = new();
}

public class ExamAssignmentCreateModel
{
    public int RoomId { get; set; }
    public int TeacherId { get; set; }
    public int AssignedStudents { get; set; }
}

public class ExamUpdateModel
{
    public int ExamDetailId { get; set; }
    public int SubjectId { get; set; }
    public int TermId { get; set; }
    public int GradeId { get; set; }
    public string ExamDate { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public List<ExamAssignmentCreateModel> Assignments { get; set; }
}
