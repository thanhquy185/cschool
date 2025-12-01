using System;

namespace cschool.Models;

public class SubjectClassModel
{
    public int Assign_class_id{ get; set; }
    public int SubjectId { get; set; }
    public int ClassId { get; set; }
    public string SubjectName { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string TermName { get; set; } = "";
    public int QuizCount { get; set; } = 0;
    public int OralCount { get; set; } = 0;
}