using System;
using System.Collections.Generic;
using System.Data;
using Avalonia.Remote.Protocol;
using cschool.Models;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Linq;

namespace cschool.Services;

public class HomeClassService
{
    private readonly DBService _db;

    public HomeClassService(DBService db)
    {
        _db = db;
    } 

    public List<Subjects> GetSubject(int assignClassID)
    {
        try
        {
            List<Subjects> ds = new List<Subjects>();
            string sql = @"SELECT subject_id,name
                            FROM assign_class_teachers 
                            JOIN subjects ON subjects.id = assign_class_teachers.subject_id
                            JOIN assign_classes ON assign_classes.id = assign_class_teachers.assign_class_id
                            WHERE assign_class_teachers.assign_class_id = @classId ";

            var connection = _db.GetConnection();
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@classId", assignClassID);

            var reader = command.ExecuteReader();
            while(reader.Read())
            {
                ds.Add(new Subjects(
                    (int)reader["subject_id"],
                    reader["name"].ToString()!
                ));
            }

            return ds;
        }catch(Exception ex)
        {
            Console.WriteLine("Lỗi không thể lấy dữ liệu: " + ex);
            return new List<Subjects>();
        }
        
    }

    public List<Student> GetStudent(int assignClassId)
    {
        try
        {
            List<Student> ds = new List<Student>();
            string sql = @"SELECT id,fullname
                            FROM students
                            JOIN assign_class_students ON student_id = students.id
                            WHERE assign_class_id = @assignClassId";
            var connection = _db.GetConnection();
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@assignClassId", assignClassId);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ds.Add(new Student(
                    (int)reader["id"],
                    reader["fullname"].ToString()!
                ));
            }
            return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi không thể lấy dữ liệu: " + ex);
            return new List<Student>();
        }
    }
    
    public List<Score> GetScores(int assignClassId)
    {
        try
        {
            List<Score> ds = new List<Score>();
           
            string sql = @"SELECT student_id, subject_id,score
                           FROM subject_term_avg
                           WHERE assign_class_id = @assignClassId";
            var connection = _db.GetConnection();
            var cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@assignClassId", assignClassId);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ds.Add(new Score(
                    (int)reader["student_id"],

                    (int)reader["subject_id"],

                    (float)reader["score"]
                ));
            }
             return ds;
        }catch(Exception ex)
        {
            Console.WriteLine("Lỗi không thể truy vấn: " + ex);
            return new List<Score>();
        }
    }

    public List<Statistical> GetGpaData(int assignClassId)
    {
        try
        {
            List<Statistical> ds = new List<Statistical>();
            string sql = @"SELECT student_id, gpa, conduct_level, academic
                        FROM term_gpa
                        WHERE assign_class_id = @assignClassId ";
            var connection = _db.GetConnection();
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@assignClassId", assignClassId);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ds.Add(new Statistical(
                    (int)reader["student_id"],
                    (float)reader["gpa"],
                    reader["conduct_level"].ToString()!,
                    reader["academic"].ToString()!

                ));
            }
            return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi không thể truy vấn dữ liệu", ex);
            return new List<Statistical>();
        }
    }
    

    public List<HomeClass> GetStudents( int assignClassId)
{
    var result = new List<HomeClass>();

    var subjects = GetSubject(assignClassId); // return List<(id,name)>
    var students = GetStudent(assignClassId); // return List<(id,name)>
    var scores = GetScores(assignClassId);     // return List<(student_id,subject_id,score)>
    var gpaData = GetGpaData(assignClassId);   // return List<(student_id,gpa,conduct,academic)>

    foreach (var stu in students)
    {
        var row = new HomeClass
        {
            StudentId = stu.Id,
            StudentName = stu.Fullname
        };

        // khởi tạo tất cả môn = null
        foreach (var sbj in subjects)
            row.SubjectScores[sbj.Name] = null;

        // gán điểm từng môn
        foreach (var sc in scores.Where(x => x.Student_id == stu.Id))
        {
            var sbjName = subjects.First(s => s.Id == sc.Subject_id).Name;
            row.SubjectScores[sbjName] = sc.ScoreSubject;
        }

        // gán gpa, conduct, academic
        var gpa = gpaData.FirstOrDefault(x => x.Student_id == stu.Id);
        if (gpa != null)
        {
            row.GpaTotal = gpa.Gpa;
            row.ConductLevel = gpa.ConductLevel;
            row.Academic = gpa.Academic;
        }

        result.Add(row);
    }

    return result;
}
}