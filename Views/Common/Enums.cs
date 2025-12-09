// Dialog Mode Enum
enum DialogModeEnum
{
    Info,
    Create,
    Update,
    Lock,
    Excel,
    ChangePassword
}

// Function Id
enum FunctionIdEnum
{
    HomeClass = 1,
    Attendance = 2,
    SubjectClass = 3,
    Statistical = 4,
    AssignTeacher = 5,
    Exam = 6,
    Tuition = 7,
    Class = 8,
    Teacher = 9,
    Student = 10,
    Role = 11,
    User = 12,
}

// // Common Status Enum
// public record CommonStatusEnum(string Value)
// {
//     public static readonly CommonStatusEnum Active = new("Hoạt động");
//     public static readonly CommonStatusEnum Inactive = new("Tạm dừng");

//     public override string ToString() => Value;
// }