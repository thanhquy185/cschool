
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
    public AssignTeacher(int assign_class_id, int teacher_id, int subject_id, string courseName, string className, string teacher, string room, string day, int start, int end)
    {
        Assign_class_id = assign_class_id;
        Teachers_id = teacher_id;
        Subject_id = subject_id;
        CourseName = courseName;
        ClassName = className;
        Teachers = teacher;
        RoomName = room;
        Day = day;
        Start = start;
        End = end;

    }
        public AssignTeacher(int assign_class_id, int teacher_id, string courseName,int quiz_count,int oral_count, string className, string teacher, string room, string day, int start, int end)
    {
        Assign_class_id = assign_class_id;
        Teachers_id = teacher_id;
        CourseName = courseName;
        QuizCount = quiz_count;
        OralCount = oral_count;
        ClassName = className;
        Teachers = teacher;
        RoomName = room;
        Day = day;
        Start = start;
        End = end;
        
    }


    public AssignTeacher(int assign_class_id, int teacher_id, int subject_id, string courseName, string className, string teacher, string room, string day, int start, int end,int quiz_connt, int oral_count)
    {
        Assign_class_id = assign_class_id;
        Teachers_id = teacher_id;
        Subject_id = subject_id;
        CourseName = courseName;
        ClassName = className;
        Teachers = teacher;
        RoomName = room;
        Day = day;
        Start = start;
        End = end;
        QuizCount = quiz_connt;
        OralCount = oral_count;

    }


}