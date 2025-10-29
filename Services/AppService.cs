using System;
using cschool.Services;

public static class AppService
{
    public static DBService? DBService { get; set; }
    public static UserService? UserService { get; set; }
    public static AssignTeacherService? AssignTeacher { get; set; }
    public static StatisticalService? statisticalService { get; set; }
    public static HomeClassService? homeClassService{ get; set; }
    public static string AppPath { get; } = AppContext.BaseDirectory.Replace("/bin/Debug/net9.0", "");
    public static string QuestionIconPath { get; } = $"{AppPath}/Assets/Images/Others/question-icon.png";
    public static AssignTeacherService? AssignTeacherService { get;  set; }
}
