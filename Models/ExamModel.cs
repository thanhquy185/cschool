using System;

public class ExamModel
{
    public int Id { get; set; }
    public string Room { get; set; }
    public string Subject { get; set; }
    public string TeacherName { get; set; }
    public string ExamDate { get; set; }
    public string TermName { get; set; }
    public string CandidateCount { get; set; }
    public string StudentName { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string ExamDateDisplay
    {
        get
        {
            if (DateTime.TryParse(ExamDate, out var date))
                return date.ToString("dd/MM/yyyy");
            return ExamDate;
        }
    }
}
