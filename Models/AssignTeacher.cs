
using System.Dynamic;

namespace cschool.Models;

public class AssignTeacher
{

    public int Assign_class_id { get; set; }
    public int Teachers_id { get; set; }
    public int Subject_id { get; set; }
    public string CourseName { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string Teachers { get; set; } = "";
    public int QuizCount { get; set; }
    public int OralCount { get; set; }
    public string Thu { get; set; } = "";
    public string Day { get; set; } = "";
    public string RoomName { get; set; } = "";
    public int ClassId { get; internal set; }
    public int Start { get; set; }
    public int End { get; set; }
    public int term_id { get; set; }
    


}