using System;
using System.Collections.Generic;
using System.Data;
using Avalonia.Remote.Protocol;
using cschool.Models;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace cschool.Services;

public class HomeClassService
{
    private readonly DBService _db;

    public HomeClassService(DBService db)
    {
        _db = db;
    } 

public List<Models.Information> GetInformation (int assignClassId)
    {
        try
        {
            List<Models.Information> ds = new List<Models.Information>();
            string sql = @"SELECT a.id,a.class_id, a.head_teacher_id, a.term_id, t.fullname, c.name as nameClass,tr.name as nameTerm, tr.year
                        FROM assign_classes a 
                        JOIN teachers t ON t.id = a.head_teacher_id
                        JOIN classes c ON c.id = a.class_id
                        JOIN terms tr ON tr.id = a.term_id
                        WHERE a.id = @assignClassId";
            var connection = _db.GetConnection();
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@assignClassId", assignClassId);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ds.Add(new Models.Information
                {
                    NameTeacher = reader["fullname"].ToString()!,
                    NameClass = reader["nameClass"].ToString()!,
                    NameTerm = reader["nameTerm"].ToString()!,
                    Year = (int)reader["year"]
                });
            }
            return ds;
        }catch(Exception e)
        {
            Console.WriteLine("Lỗi không lấy được thông tin:" + e);
            return new List<Models.Information>();
        }
    }



    public List<HomeClass> GetStudents(int assignClassId)
    {
        try
        {
            List<HomeClass> rawList = new List<HomeClass>();

            string sql = @"
                SELECT 
                    st.fullname,st.id as studentId, 
                    s.name AS subject_name,
                    sta.score AS subject_score,
                    tg.gpa,
                    tg.conduct_level,
                    tg.academic
                FROM students st
                JOIN subject_term_avg sta ON sta.student_id = st.id
                JOIN subjects s ON s.id = sta.subject_id
                JOIN term_gpa tg ON tg.student_id = st.id
                WHERE sta.assign_class_id = @assignClassId
                ORDER BY st.fullname, s.name";

            var connection = _db.GetConnection();
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@assignClassId", assignClassId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    rawList.Add(new HomeClass
                    {
                        StudentId = (int)reader["studentId"],
                        StudentName = reader["fullname"].ToString()!,
                        SubjectName = reader["subject_name"].ToString()!,
                        GpaSubject = Convert.ToSingle(reader["subject_score"]),
                        GpaTotal = Convert.ToSingle(reader["gpa"]),
                        ConductLevel = reader["conduct_level"].ToString()!,
                        Academic = reader["academic"].ToString()!
                    });
                }
            }

            // 🧩 Gom nhóm theo học sinh
       var grouped = rawList
            .GroupBy(x => new { x.StudentId, x.StudentName }) // Group by cả ID và Name
            .Select(g => new HomeClass
            {
                StudentId = g.Key.StudentId,
                StudentName = g.Key.StudentName, // THÊM DÒNG NÀY
                GpaTotal = g.First().GpaTotal,
                ConductLevel = g.First().ConductLevel,
                Academic = g.First().Academic,
                // Ghép danh sách môn và điểm thành 1 chuỗi
                SubjectName = string.Join("\n", g.Select(x => $"{x.SubjectName}: {x.GpaSubject}"))
            })
            .ToList();

        Console.WriteLine($"✅ Đã load {grouped.Count} học sinh");
        foreach (var student in grouped)
        {
            Console.WriteLine($"  - {student.StudentName} (ID: {student.StudentId})");
        }

        return grouped;
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Lỗi không thể lấy dữ liệu: " + ex.Message);
        return new List<HomeClass>();
    }
    }
    public List<DetailScore> GetDetailScores1(int id)
    {
        try
        {
                 Console.WriteLine($"=== DEBUG GetDetailScores1 ===");
                Console.WriteLine($"Student ID: {id}");
            List<DetailScore> ds = new List<DetailScore>();
            string sql = @" SELECT s.name as nameSubject, sd.score 
                            FROM score_details sd
                            JOIN subjects s ON s.id = sd.subject_id
                            WHERE sd.exam_type_id=1 AND sd.student_id = @studentId ";
            var connection = _db.GetConnection();
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@studentId", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ds.Add(new DetailScore
                {
                    NameSubject = reader["nameSubject"].ToString()!,
                    DiemMieng = Convert.ToSingle(reader["score"])
                });
            }

            return ds;
        } catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể lấy chi tiết" + e);
            return new List<DetailScore>();
        }

    }
    public List<DetailScore> GetDetailScores2(int id)
    {
        try
        {
            List<DetailScore> ds = new List<DetailScore>();
            string sql = @"SELECT s.name as nameSubject, sd.score 
                            FROM score_details sd
                            JOIN subjects s ON s.id = sd.subject_id
                            WHERE sd.exam_type_id=2 AND sd.student_id = @studentId ";
            var connection = _db.GetConnection();
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@studentId", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ds.Add(new DetailScore
                {
                    NameSubject = reader["nameSubject"].ToString()!,
                    Diem15p = Convert.ToSingle(reader["score"])
                });
            }

            return ds;
        } catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể lấy chi tiết" + e);
            return new List<DetailScore>();
        }

    }
    public List<DetailScore> GetDetailScores3(int id)
    {
         try
        {
            List<DetailScore> ds = new List<DetailScore>();
            string sql = @"SELECT s.name as nameSubject, sd.score 
                            FROM score_details sd
                            JOIN subjects s ON s.id = sd.subject_id
                            WHERE sd.exam_type_id = 3 AND sd.student_id = @studentId ";
            var connection = _db.GetConnection();
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@studentId", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ds.Add(new DetailScore
                {
                    NameSubject = reader["nameSubject"].ToString()!,
                    DiemGK = Convert.ToSingle(reader["score"])
                });
            }

            return ds;
        } catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể lấy chi tiết" + e);
            return new List<DetailScore>();
        }


    }
    public List<DetailScore> GetDetailScores4(int id)
    {
         try
        {
            List<DetailScore> ds = new List<DetailScore>();
            string sql = @"SELECT s.name as nameSubject, sd.score 
                            FROM score_details sd
                            JOIN subjects s ON s.id = sd.subject_id
                            WHERE sd.exam_type_id = 4 AND sd.student_id = @studentId ";
            var connection = _db.GetConnection();
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@studentId", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ds.Add(new DetailScore
                {
                    NameSubject = reader["nameSubject"].ToString()!,
                    DiemCK = Convert.ToSingle(reader["score"])
                });
            }

            return ds;
        } catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể lấy chi tiết" + e);
            return new List<DetailScore>();
        }
       

    }
  public List<HomeClass> Search(int assignClassId, string name)
    {
        try
        {
            List<HomeClass> rawList = new List<HomeClass>();

            string sql = @"
                SELECT 
                    st.fullname, 
                    s.name AS subject_name,
                    sta.score AS subject_score,
                    tg.gpa,
                    tg.conduct_level,
                    tg.academic
                FROM students st
                JOIN subject_term_avg sta ON sta.student_id = st.id
                JOIN subjects s ON s.id = sta.subject_id
                JOIN term_gpa tg ON tg.student_id = st.id
                WHERE sta.assign_class_id = @assignClassId AND st.fullname LIKE @nameStudent
                ORDER BY st.fullname, s.name";

            var connection = _db.GetConnection();
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@assignClassId", assignClassId);
            cmd.Parameters.AddWithValue("@nameStudent", $"%{name}%");

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    rawList.Add(new HomeClass
                    {
                        StudentName = reader["fullname"].ToString()!,
                        SubjectName = reader["subject_name"].ToString()!,
                        GpaSubject = Convert.ToSingle(reader["subject_score"]),
                        GpaTotal = Convert.ToSingle(reader["gpa"]),
                        ConductLevel = reader["conduct_level"].ToString()!,
                        Academic = reader["academic"].ToString()!
                    });
                }
            }

            // 🧩 Gom nhóm theo học sinh
            var grouped = rawList
                .GroupBy(x => x.StudentName)
                .Select(g => new HomeClass
                {
                    StudentName = g.Key,
                    GpaTotal = g.First().GpaTotal,
                    ConductLevel = g.First().ConductLevel,
                    Academic = g.First().Academic,
                    // Ghép danh sách môn và điểm thành 1 chuỗi
                    SubjectName = string.Join("\n", g.Select(x => $"{x.SubjectName}: {x.GpaSubject}"))
                })
                .ToList();

            return grouped;
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Lỗi không thể lấy dữ liệu: " + ex.Message);
            return new List<HomeClass>();
        }
}

}