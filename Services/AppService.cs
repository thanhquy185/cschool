using Services;

public static class AppService
{
    public static DBService DBService { get; set; }
    public static AssignTeacherService? AssignTeacher { get; set; }
    public static AssignTeacherService? AssignTeacherService { get; set; }
    public static ExamService ExamService { get; set; }
    public static TuitionService TuitionService { get; set; }
    public static StatisticalService? StatisticalService { get; set; }
    public static HomeClassService? HomeClassService { get; set; }
    public static SubjectClassService? SubjectClassService { get; set; }
    public static TeacherService? TeacherService { get; set; }
    public static StudentService StudentService { get; set; }
    public static FunctionService FunctionService { get; set; }
    public static UserService UserService { get; set; }
    public static RoleService RoleService { get; set; }
    public static RoleDetailService RoleDetailService { get; set; }
    public static string RootPath { get; } = $"{AppPath}/Assets/Images/";
    public static string AppPath { get; } = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    public static string QuestionIconPath { get; } = $"{AppPath}/Assets/Images/Others/question-icon.png";

}