using System.Data;
using Models;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.ComponentModel;

namespace Services;

public class AssignTeacherService
{
    private readonly DBService _db;

    public AssignTeacherService(DBService db)
    {
        _db = db;
    }
    public List<Subjects> GetCourses()
    {
        try
        {
            var ds = new List<Subjects>();
            string sql = "SELECT id, name FROM subjects Where status = 1";
            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow data in dt.Rows)
            {
                ds.Add(new Subjects(
                    (int)data["id"],
                    data["name"].ToString()!
                ));
            }
            return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi không thể lấy dữ liệu môn học: " + ex);
            return new List<Subjects>();
        }
    }
    public bool IsClassBusy(int assignClassId, string day, int start, int end)
{
    try
    {
        string sql = @"
        SELECT COUNT(*)
        FROM assign_class_teachers
        JOIN assign_classes ac ON ac.id = assign_class_teachers.assign_class_id
        WHERE assign_class_id = @assignClassId AND ac.term_id = (SELECT  term_id FROM assign_classes WHERE id = @assignClassId1 LIMIT 1)
          AND day = @day      
          AND (
                (@start BETWEEN start_period AND end_period)
                OR (@end BETWEEN start_period AND end_period)
                OR (start_period BETWEEN @start AND @end)
                OR (end_period BETWEEN @start AND @end)
              )";

        using var conn = _db.GetConnection();
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@assignClassId", assignClassId);
        cmd.Parameters.AddWithValue("@assignClassId1", assignClassId);
        cmd.Parameters.AddWithValue("@day", day);
        cmd.Parameters.AddWithValue("@start", start);
        cmd.Parameters.AddWithValue("@end", end);

        int count = Convert.ToInt32(cmd.ExecuteScalar());
        Console.WriteLine($"DEBUG IsClassBusy - ClassId: {assignClassId}, Day: {day}, Start: {start}, End: {end}, Conflicts: {count}");
        
        return count > 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi kiểm tra trùng lịch lớp: {ex.Message}");
        return false;
    }
}

    public bool IsTeacherBusy(int teacherId, string day, int start, int end,int assign_class_id)
    {
        string sql = @"
        SELECT COUNT(*)
        FROM assign_class_teachers
        JOIN assign_classes ac ON ac.id = assign_class_teachers.assign_class_id
        WHERE teacher_id = @teacherId AND ac.term_id = (SELECT  term_id FROM assign_classes WHERE id = @assign_class_id LIMIT 1)
          AND day = @day      
          AND (
                (@start BETWEEN start_period AND end_period)
                OR (@end BETWEEN start_period AND end_period)
                OR (start_period BETWEEN @start AND @end)
                OR (end_period BETWEEN @start AND @end)
              )";

        using var conn = _db.GetConnection();
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@teacherId", teacherId);
        cmd.Parameters.AddWithValue("@assign_class_id", assign_class_id);
        cmd.Parameters.AddWithValue("@day", day);
        // cmd.Parameters.AddWithValue("@assign_class_id",assign_class_id);
        cmd.Parameters.AddWithValue("@start", start);
        cmd.Parameters.AddWithValue("@end", end);

        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }
public bool IsConflict(AssignTeacher at)
{
    var connection = _db.GetConnection();

    string sql = @"
        SELECT COUNT(*) FROM assign_class_teachers
        JOIN assign_classes ac ON ac.id = assign_class_teachers.assign_class_id
        WHERE teacher_id = @teacher_id
          AND day = @day
          AND ac.term_id = (SELECT term_id FROM assign_classes WHERE id = @assign_class_id1  LIMIT 1)
          AND assign_class_id != @assign_class_id
          AND (
                (start_period < @end_period AND end_period > @start_period)
              )";

    using var cmd = new MySqlCommand(sql, connection);
    cmd.Parameters.AddWithValue("@teacher_id", at.Teachers_id);
    cmd.Parameters.AddWithValue("@day", at.Day);
    cmd.Parameters.AddWithValue("@assign_class_id1", at.Assign_class_id);
    cmd.Parameters.AddWithValue("@assign_class_id", at.Assign_class_id);
    cmd.Parameters.AddWithValue("@start_period", at.Start);
    cmd.Parameters.AddWithValue("@end_period", at.End);

    int count = Convert.ToInt32(cmd.ExecuteScalar());
    return count > 0;
}

    public List<Classes> GetClasses()
    {
        try
        {
            var ds = new List<Classes>();
            string sql = @"SELECT c.id as class_id, c.name,c.area,c.room, ac.class_id, ac.id as assign_class_id
                    FROM classes c
                    JOIN assign_classes ac ON c.id = ac.class_id
                    WHERE ac.term_id = (SELECT MAX(term_id) FROM assign_classes)";
            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow data in dt.Rows)
            {
                ds.Add(new Classes(
                    (int)data["class_id"],
                    (int)data["assign_class_id"],
                    data["name"].ToString()!,
                    data["room"].ToString()!
                ));
            }
            return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi không thể lấy dữ liệu lớp học: " + ex);
            return new List<Classes>();
        }
    }


    public BindingList<TeacherModel> GetTeachers(int id)
    {
        try{
        var ds = new BindingList<TeacherModel>();
        string sql = @" select t.id , t.fullname, d.name as department_name
         FROM teachers t
         JOIN department_details dd ON dd.teacher_id = t.id
         JOIN departments d ON d.id = dd.department_id 
         WHERE d.subject_id = "+ id;

        var dt = _db.ExecuteQuery(sql);

        foreach (DataRow data in dt.Rows)
        {
            ds.Add(new TeacherModel{
                Id = (int)data["id"],
                Name = data["fullname"].ToString()!,
                DepartmentName = data["department_name"].ToString()!

        });

        }
        return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi không thể lấy dữ liệu giáo viên: " + ex);
            return new BindingList<TeacherModel>();
        }
    }

    public BindingList<TeacherModel> GetTeachers()
    {
        try{
        var ds = new BindingList<TeacherModel>();
        string sql = @" select t.id , t.fullname, d.name as department_name
         FROM teachers t
         JOIN department_details dd ON dd.teacher_id = t.id
         JOIN departments d ON d.id = dd.department_id 
         ";

        var dt = _db.ExecuteQuery(sql);

        foreach (DataRow data in dt.Rows)
        {
            ds.Add(new TeacherModel{
                Id = (int)data["id"],
                Name = data["fullname"].ToString()!,
                DepartmentName = data["department_name"].ToString()!

        });

        }
        return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi không thể lấy dữ liệu giáo viên: " + ex);
            return new BindingList<TeacherModel>();
        }
    }


    public bool AddAssignmentTeacher(AssignTeacher courseAssignment)
    {
        try
        {

            string sql = @"INSERT INTO assign_class_teachers (assign_class_id, teacher_id, subject_id, quiz_count, oral_count, day,start_period,end_period) 
                         VALUES (@assignClassId, @teacherId, @subjectId, @quizCount, @oralCount, @day,@start_period,@end_period)";

            var connection = _db.GetConnection();
        
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@assignClassId", courseAssignment.Assign_class_id); // assuming className is the assign_class_id
            command.Parameters.AddWithValue("@teacherId", courseAssignment.Teachers_id);
            command.Parameters.AddWithValue("@subjectId", courseAssignment.Subject_id);
            command.Parameters.AddWithValue("@quizCount", courseAssignment.QuizCount);
            command.Parameters.AddWithValue("@oralCount", courseAssignment.OralCount);
            command.Parameters.AddWithValue("@day", courseAssignment.Day);
            command.Parameters.AddWithValue("@start_period", courseAssignment.Start);
            command.Parameters.AddWithValue("@end_period", courseAssignment.End);

            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("lỗi " + ex);
            return false;
        }
 
    }

    public List<AssignTeacher> GetAssignTeachers()
    {
        try
        {
            var ds = new List<AssignTeacher>();
            string sql = @"select at.assign_class_id as assignClassId,at.day,at.quiz_count,oral_count, at.start_period, at.end_period, at.teacher_id as teacherId,c.name as className, c.room as roomName,
                            t.fullname as nameTeacher, s.name as subjectName, s.id as subjectId, ac.term_id 
                            FROM assign_class_teachers at
                            JOIN assign_classes ac ON ac.id = at.assign_class_id 
                            JOIN classes c ON c.id = ac.class_id
                            JOIN subjects s ON s.id = at.subject_id
                            JOIN teachers t ON t.id = at.teacher_id
                            WHERE ac.term_id = (SELECT MAX(term_id) FROM assign_classes)
                            ";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow data in dt.Rows)
            {
                ds.Add(new AssignTeacher{
                    Assign_class_id = (int)data["assignClassId"],
                    Teachers_id =  (int)data["teacherId"],
                    Subject_id = (int)data["subjectId"],
                    CourseName =  data["subjectName"].ToString()!,
                    ClassName = data["className"].ToString()!,
                    Teachers = data["nameTeacher"].ToString()!,
                    RoomName = data["roomName"].ToString()!,
                    Day = data["day"].ToString()!,
                    Start = (int)data["start_period"],
                    End = (int)data["end_period"],
                    QuizCount = (int)data["quiz_count"],
                    OralCount = (int)data["oral_count"],
                    term_id = (int)data["term_id"]                     
            });

            }

            return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi không thể lấy dữ liệu phân công: " + ex);
            return new List<AssignTeacher>();
        }

    }

    public bool DeleteAssignTeacher(AssignTeacher at)
    {
        try
        {
            var connection = _db.GetConnection();
            string sql = "DELETE FROM assign_class_teachers WHERE assign_class_id = @assign_class_id AND teacher_id = @teacher_id ";
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@assign_class_id", at.Assign_class_id);
            command.Parameters.AddWithValue("@teacher_id", at.Teachers_id);

            return command.ExecuteNonQuery() > 0;

        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi không thể xóa" + ex);
            return false;
        }
    }

  public List<AssignTeacher> Search(string s)
{
    try
    {
        var ds = new List<AssignTeacher>();
        // SỬA LẠI QUERY
        string sql = @"SELECT at.assign_class_id as assignClassId, at.day, at.start_period, at.end_period, 
                              at.teacher_id as teacherId,at.quiz_count,oral_count, c.name as className, c.room as roomClass,
                              t.fullname as teacherName, s.name as subjectName, s.id as subjectId
                       FROM assign_class_teachers at
                       JOIN assign_classes ac ON ac.id = at.assign_class_id
                       JOIN classes c ON c.id = ac.class_id
                       JOIN subjects s ON s.id = at.subject_id
                       JOIN teachers t ON t.id = at.teacher_id
                       WHERE t.fullname LIKE @search OR c.name LIKE @search OR s.name LIKE @search AND ac.term_id = (SELECT MAX(term_id) FROM assign_classes)";

        var connection = _db.GetConnection();
        var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@search", $"%{s}%");
         // Sử dụng LIKE cho tìm kiếm
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            ds.Add(new AssignTeacher{
                   Assign_class_id=  (int)reader["assignClassId"],
                    Teachers_id = (int)reader["teacherId"],
                    Subject_id = (int)reader["subjectId"],
                    CourseName = reader["subjectName"].ToString()!,
                    ClassName = reader["className"].ToString()!,
                    Teachers = reader["teacherName"].ToString()!,
                    RoomName = reader["roomClass"].ToString()!,
                    Day = reader["day"].ToString()!,
                    Start = (int)reader["start_period"],
                    End = (int)reader["end_period"],
                    QuizCount = (int)reader["quiz_count"],
                    OralCount = (int)reader["oral_count"]
        });
        }
       
        return ds;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Lỗi không thể tìm kiếm: " + ex);
        return new List<AssignTeacher>();
    }
}
    public bool Update(AssignTeacher at)
    {
        try
        {
            Console.WriteLine("Updating AssignTeacher with ID: " + at.Assign_class_id + ", Teacher ID: " + at.Teachers_id + ", Subject ID: " + at.Subject_id);
            var connection = _db.GetConnection();
            string sql = @"UPDATE assign_class_teachers SET 
                            quiz_count = @quiz_count,
                            oral_count = @oral_count, day = @day, start_period = @start_period, end_period = @end_period
                            WHERE assign_class_id = @assign_class_id AND teacher_id = @teacher_id AND subject_id = @subject_id";
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@subject_id", at.Subject_id);
            command.Parameters.AddWithValue("@quiz_count", at.QuizCount);
            command.Parameters.AddWithValue("@oral_count", at.OralCount);
            command.Parameters.AddWithValue("@day", at.Day);
            command.Parameters.AddWithValue("@start_period", at.Start);
            command.Parameters.AddWithValue("@end_period", at.End);
            command.Parameters.AddWithValue("@assign_class_id", at.Assign_class_id);
            command.Parameters.AddWithValue("@teacher_id", at.Teachers_id);
            // command.Parameters.AddWithValue("@subject_id", at.Subject_id);


            
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception ex)
        {
            Console.Write("Lỗi không thể cập nhật" + ex);
            return false;
        }
        
        
    }

   public List<string> GetDaysOfWeek(DateTime date)
    {
        // Dùng ngôn ngữ tiếng Việt để hiển thị "Thứ Hai", "Chủ nhật", ...
        CultureInfo viCulture = new CultureInfo("vi-VN");

        // Tìm ngày đầu tuần (Thứ Hai)
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateTime monday = date.AddDays(-diff);

        // Danh sách kết quả
        List<string> daysInWeek = new List<string>();

        // Lặp qua 7 ngày (Thứ Hai → Chủ Nhật)
        for (int i = 0; i < 7; i++)
        {
            DateTime day = monday.AddDays(i);
            string thu = day.ToString("dddd", viCulture);  // Thứ
            string ngay = day.ToString("dd/MM/yyyy");      // Ngày
            daysInWeek.Add($"{thu}");
        }

        return daysInWeek;
    }
}