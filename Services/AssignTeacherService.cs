using System;
using System.Collections.Generic;
using System.Data;
using Avalonia.Remote.Protocol;
using cschool.Models;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Formats.Asn1;

namespace cschool.Services;

public class AssignTeacherService
{
    private readonly DBService _db;

    public AssignTeacherService(DBService db)
    {
        _db = db;
    }
    public List<Subjects> GetCourses()
    {
        try {
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
    public List<Classes> GetClasses()
    {
        try
        {
            var ds = new List<Classes>();
            string sql = @"SELECT c.id as class_id, c.name,c.area,c.room, ac.class_id, ac.id as assign_class_id
                    FROM classes c
                    JOIN assign_classes ac ON c.id = ac.class_id ";

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


    public List<TeacherModel> GetTeachers()
    {
        try{
        var ds = new List<TeacherModel>();
        string sql = @" select t.id , t.fullname, d.name as department_name
         FROM teachers t
         JOIN department_details dd ON dd.teacher_id = t.id
         JOIN departments d ON d.id = dd.department_id ";

        var dt = _db.ExecuteQuery(sql);

        foreach (DataRow data in dt.Rows)
        {
            ds.Add(new TeacherModel
            {
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
            return new List<TeacherModel>();
        }
    }

    public bool AddAssignmentTeacher(AssignTeacher courseAssignment)
    {
        try
        {
            string sql = @"INSERT INTO assign_class_teachers (assign_class_id, teacher_id, subject_id, quiz_connt, oral_count, day,start_period,end_period) 
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
            string sql = @"select at.assign_class_id as assignClassId,at.day,at.quiz_connt,oral_count, at.start_period, at.end_period, at.teacher_id as teacherId,c.name as className, c.room as roomName,
        t.fullname as nameTeacher, s.name as subjectName
        FROM assign_class_teachers at
        JOIN assign_classes ac ON ac.id = at.assign_class_id 
        JOIN classes c ON c.id = ac.class_id
        JOIN subjects s ON s.id = at.subject_id
        JOIN teachers t ON t.id = at.teacher_id";

            var dt = _db.ExecuteQuery(sql);
            foreach (DataRow data in dt.Rows)
            {
                ds.Add(new AssignTeacher(
                    (int)data["assignClassId"],
                    (int)data["teacherId"],
                    data["subjectName"].ToString()!,
                    (int)data["quiz_connt"],
                    (int)data["oral_count"],
                    data["className"].ToString()!,
                    data["nameTeacher"].ToString()!,
                    data["roomName"].ToString()!,
                    data["day"].ToString()!,
                    (int)data["start_period"],
                    (int)data["end_period"]

                ));

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
            command.Parameters.AddWithValue("@assign_class_id ", at.Assign_class_id);
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
                              at.teacher_id as teacherId,at.quiz_connt,oral_count, c.name as className, c.room as roomClass,
                              t.fullname as teacherName, s.name as subjectName, s.id as subjectId
                       FROM assign_class_teachers at
                       JOIN assign_classes ac ON ac.id = at.assign_class_id
                       JOIN classes c ON c.id = ac.class_id
                       JOIN subjects s ON s.id = at.subject_id
                       JOIN teachers t ON t.id = at.teacher_id
                       WHERE t.name LIKE @search OR c.name LIKE @search OR s.name LIKE @search";

        var connection = _db.GetConnection();
        var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@search", $"%{s}%"); // Sử dụng LIKE cho tìm kiếm
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            ds.Add(new AssignTeacher(
                (int)reader["assignClassId"],
                (int)reader["teacherId"],
                reader["subjectName"].ToString()!,
                (int)reader["quiz_connt"],
                (int)reader["oral_count"],
                reader["className"].ToString()!,
                reader["teacherName"].ToString()!,
                reader["roomClass"].ToString()!,
                reader["day"].ToString()!,
                (int)reader["start_period"],
                (int)reader["end_period"]
            ));
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
            var connection = _db.GetConnection();
            string sql = @"UPDATE assign_class_teachers SET 
                            subject_id = @subject_id, quiz_connt = @quiz_connt,
                            oral_count = @oral_count, day = @day, start_period = @start_period, end_period = @end_period
                            WHERE assign_class_id = @assign_class_id AND teacher_id = @teacher_id ";
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@subject_id", at.Subject_id);
            command.Parameters.AddWithValue("@quiz_connt", at.QuizCount);
            command.Parameters.AddWithValue("@oral_count", at.OralCount);
            command.Parameters.AddWithValue("@day", at.Day);
            command.Parameters.AddWithValue("@start_period", at.Start);
            command.Parameters.AddWithValue("@end_period", at.End);
            command.Parameters.AddWithValue("@assign_class_id", at.Assign_class_id);
            command.Parameters.AddWithValue("@teacher_id", at.Teachers_id);


            return command.ExecuteNonQuery() > 1;
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
            daysInWeek.Add($"{thu} - {ngay}");
        }

        return daysInWeek;
    }
}