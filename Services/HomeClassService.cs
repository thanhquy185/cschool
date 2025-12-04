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
                JOIN term_gpa tg ON tg.student_id = st.id AND tg.assign_class_id = @assignClassId1
                WHERE sta.assign_class_id = @assignClassId
                ORDER BY st.fullname, s.name";

            var connection = _db.GetConnection();
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@assignClassId1", assignClassId);
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
                SubjectName = string.Join("\n", g.Select(x => $"{x.SubjectName}: {x.GpaSubject}")),
                GpaSubject = g.First().GpaSubject
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
        
        // Dictionary để nhóm điểm theo môn
        var subjectScores = new Dictionary<string, List<float>>();
        
        string sql = @"SELECT s.name as nameSubject, sd.score 
                      FROM score_details sd
                      JOIN subjects s ON s.id = sd.subject_id
                      WHERE sd.exam_type_id=1 AND sd.student_id = @studentId";
        
        var connection = _db.GetConnection();
        var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@studentId", id);
        var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            string subjectName = reader["nameSubject"].ToString()!;
            float score = Convert.ToSingle(reader["score"]);
            
            if (!subjectScores.ContainsKey(subjectName))
            {
                subjectScores[subjectName] = new List<float>();
            }
            subjectScores[subjectName].Add(score);
        }
        reader.Close();

        // Chuyển đổi dictionary thành List<DetailScore>
        List<DetailScore> result = new List<DetailScore>();
        foreach (var subject in subjectScores)
        {
            result.Add(new DetailScore
            {
                NameSubject = subject.Key,
                DiemMieng = subject.Value
            });
        }

        Console.WriteLine($"Đã load {result.Count} môn có điểm miệng");
        foreach (var item in result)
        {
            Console.WriteLine($"  - {item.NameSubject}: {string.Join(", ", item.DiemMieng)}");
        }
        
        return result;
    }
    catch (Exception e)
    {
        Console.WriteLine("Lỗi không thể lấy chi tiết" + e);
        return new List<DetailScore>();
    }
}

public List<DetailScore> GetDetailScores2(int id)
{
    try
    {
        // Dictionary để nhóm điểm theo môn
        var subjectScores = new Dictionary<string, List<float>>();
        
        string sql = @"SELECT s.name as nameSubject, sd.score 
                      FROM score_details sd
                      JOIN subjects s ON s.id = sd.subject_id
                      WHERE sd.exam_type_id=2 AND sd.student_id = @studentId";
        
        var connection = _db.GetConnection();
        var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@studentId", id);
        var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            string subjectName = reader["nameSubject"].ToString()!;
            float score = Convert.ToSingle(reader["score"]);
            
            if (!subjectScores.ContainsKey(subjectName))
            {
                subjectScores[subjectName] = new List<float>();
            }
            subjectScores[subjectName].Add(score);
        }
        reader.Close();

        // Chuyển đổi dictionary thành List<DetailScore>
        List<DetailScore> result = new List<DetailScore>();
        foreach (var subject in subjectScores)
        {
            result.Add(new DetailScore
            {
                NameSubject = subject.Key,
                Diem15p = subject.Value
            });
        }

        Console.WriteLine($"Đã load {result.Count} môn có điểm 15p");
        foreach (var item in result)
        {
            Console.WriteLine($"  - {item.NameSubject}: {string.Join(", ", item.Diem15p)}");
        }
        
        return result;
    }
    catch (Exception e)
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

    public bool Update(int studentId, string ConductLevel)
{
    try
    {
        string academic = "Trung bình";
        float gpaTotal = 0;

        using (var conn = _db.GetConnection())
        {
        

            // --- 1. Lấy GPA ---
            string sql1 = "SELECT gpa FROM term_gpa WHERE student_id = @studentId1";
            using (var cmd = new MySqlCommand(sql1, conn))
            {
                cmd.Parameters.AddWithValue("@studentId1", studentId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        gpaTotal = Convert.ToSingle(reader["gpa"]);
                    }
                } // reader đóng ở đây
            }

            // --- 2. Tính academic ---
            if (ConductLevel == "Giỏi" && gpaTotal >= 8)
            {
                academic = "Giỏi";
            }
            else if ((ConductLevel == "Giỏi" || ConductLevel == "Khá") &&
                     ((gpaTotal < 8 && gpaTotal >= 6.5) || (gpaTotal < 6.5 && gpaTotal >= 5)) || (ConductLevel == "Khá" && gpaTotal >= 8 ))
            {
                academic = "Khá";
            }

            // --- 3. UPDATE term_gpa ---
            string sql = "UPDATE term_gpa SET conduct_level = @conductLevel, academic = @academic WHERE student_id = @studentId";
            using (var cmd2 = new MySqlCommand(sql, conn))
            {
                cmd2.Parameters.AddWithValue("@conductLevel", ConductLevel);
                cmd2.Parameters.AddWithValue("@academic", academic);
                cmd2.Parameters.AddWithValue("@studentId", studentId);

                return cmd2.ExecuteNonQuery() > 0;
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("Không thể cập nhật hành kiểm: " + e.Message);
        return false;
    }
}

}