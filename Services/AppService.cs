using System;
using cschool.Services;

public static class AppService
{
    public static DBService DBService { get; set; }
    public static UserService UserService { get; set; }
    public static StudentService StudentService { get; set; }
    public static ExamService ExamService { get; set; }
    public static TuitionService TuitionService { get; set; }
    public static string AppPath { get; } = AppContext.BaseDirectory.Replace("/bin/Debug/net9.0", "");
    public static string QuestionIconPath { get; } = $"{AppPath}/Assets/Images/Others/question-icon.png";
}
