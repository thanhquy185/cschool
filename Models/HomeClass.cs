using System;
using System.Collections.Generic;

namespace cschool.Models;

public class HomeClass
{
    public int Id { get; set; }
    public int Class_id { get; set; }
    public int Head_teacher_id { get; set; }
    public string ClassName { get; set; } = "";
    public string TeacherName { get; set; } = "";
    public int Term_id { get; set; }
    public string LearnYear { get; set; } = "";
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";

    // key = subject_name, value = score
    public Dictionary<string, float?> SubjectScores { get; set; } = new();

    public float GpaTotal { get; set; }
    public string ConductLevel { get; set; } = "";
    public string Academic { get; set; } = "";
   


}
