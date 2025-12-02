using System;
using System.Collections.Generic;
using System.Data;
using cschool.Models;
using MySql.Data.MySqlClient;

namespace cschool.Services;

public class TeacherService
{
    // Phương thức và thuộc tính liên quan đến quản lý giáo viên sẽ được định nghĩa ở đây.

    private readonly DBService _db;
    public TeacherService(DBService dbService)
    {
        _db = dbService;
    }

    public List<TeacherModel> GetTeachers()
    {
        try
        {
            List<TeacherModel> ds = new List<TeacherModel>();
            string sql = @" SELECT t.id, t.avatar, t.fullname, t.birthday, t.gender, t.address, t.phone, t.email, 
                            COALESCE(dd.department_id, 0) AS department_id, 
                            COALESCE(d.name, 'Chưa có bộ môn') AS department_name, 
                            t.status, 
                            COALESCE(ac.class_id, 0) AS class_id, 
                            COALESCE(c.name, 'Chưa có lớp') AS class_name
                            FROM teachers t
                            LEFT JOIN department_details dd ON dd.teacher_id = t.id
                            LEFT JOIN departments d ON d.id = dd.department_id
                            LEFT JOIN assign_class_teachers act ON act.teacher_id = t.id
                            LEFT JOIN assign_classes ac ON ac.id = act.assign_class_id
                            LEFT JOIN classes c ON c.id = ac.class_id
                            WHERE t.status = 1";

            var result = _db.ExecuteQuery(sql);
            foreach (DataRow data in result.Rows)
            {
                ds.Add(new TeacherModel{
                    Id = Convert.ToInt32(data["id"]),
                    Avatar = data["avatar"].ToString()!,
                    Name = data["fullname"].ToString()!,
                    Gender = data["gender"].ToString()!,
                    Birthday = Convert.ToDateTime(data["birthday"]).ToString("dd/MM/yyyy")!,
                    ClassId = Convert.ToInt32(data["class_id"]),
                    ClassName = data["class_name"].ToString()!,
                    Email = data["email"].ToString()!,
                    Phone = data["phone"].ToString()!,
                    Address = data["address"].ToString()!,
                    DepartmentId = Convert.ToInt32(data["department_id"]),
                    DepartmentName = data["department_name"].ToString()!,
                });

            }
            return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTeachers: {ex.Message}");
            return new List<TeacherModel>();
        }

    }

    public int GetIDLastTeacher()
    {
        // Console.WriteLine(123);
        var dt = _db.ExecuteQuery("SELECT id FROM cschool.teachers ORDER BY id DESC LIMIT 1");
        if (dt.Rows.Count > 0)
            return System.Convert.ToInt32(dt.Rows[0]["id"]);
        return 0;
    }

    public bool CreateTeacher(TeacherModel t)
    {
        try
        {
            var connection = _db.GetConnection();
            string sql = @"INSERT INTO teachers (avatar, fullname, birthday, gender, address, phone, email) 
                       VALUES (@avatar, @fullname, @birthday, @gender, @address, @phone, @email);
                       ";
            
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@avatar", t.Avatar);
            command.Parameters.AddWithValue("@fullname", t.Name);
            command.Parameters.AddWithValue("@birthday", t.Birthday);
            command.Parameters.AddWithValue("@gender", t.Gender);
            command.Parameters.AddWithValue("@address", t.Address);
            command.Parameters.AddWithValue("@phone", t.Phone);
            command.Parameters.AddWithValue("@email", t.Email);
            // command.Parameters.AddWithValue("@status", t.Status);
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0) return false;

            if (t.DepartmentId > 0)
            {
                string department_sql = @"INSERT INTO department_details (department_id, teacher_id) 
                                          VALUES (@department_id, LAST_INSERT_ID())";
                var departCommand = new MySqlCommand(department_sql, connection);
                departCommand.Parameters.AddWithValue("@department_id", t.DepartmentId);
                int deptRows = departCommand.ExecuteNonQuery();
                if (deptRows == 0) return false;
            }
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"lỗi không thể thêm giáo viên: {ex.Message}");
            return false;
        }
    }


    public bool LockTeacher(int id)
    {
        try
        {
            var connection = _db.GetConnection();
            string sql = @"UPDATE teachers SET status = 0 where id = @id";

            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể xóa giáo viên: " + e);
            return false;
        }
    }

    public bool UpdateTeacher(TeacherModel t)
    {
        try
        {
            var connection = _db.GetConnection();
            string sql = @"UPDATE teachers SET fullname = @fullname, birthday = @birthday, gender = @gender, address = @address,
                                  phone = @phone, email = @email, avatar = @avatar, status = @status
                           where id = @id";

            string dept_sql = @"UPDATE department_details SET department_id = @department_id WHERE teacher_id = @id";

            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@fullname", t.Name);
            command.Parameters.AddWithValue("@birthday", t.Birthday);
            command.Parameters.AddWithValue("@gender", t.Gender);
            command.Parameters.AddWithValue("@address", t.Address);
            command.Parameters.AddWithValue("@phone", t.Phone);
            command.Parameters.AddWithValue("@email", t.Email);
            command.Parameters.AddWithValue("@avatar", t.Avatar ?? "");
            command.Parameters.AddWithValue("@status", t.Status);
            command.Parameters.AddWithValue("@id", t.Id);
            
            Console.WriteLine($"🔍 UpdateTeacher: ID={t.Id}, Name={t.Name}, Avatar={t.Avatar}, Status={t.Status}");
            
            if (command.ExecuteNonQuery() > 0)
            {
                var deptCommand = new MySqlCommand(dept_sql, connection);
                deptCommand.Parameters.AddWithValue("@department_id", t.DepartmentId);
                deptCommand.Parameters.AddWithValue("@id", t.Id);
                return deptCommand.ExecuteNonQuery() > 0;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể cập nhật giáo viên: " + e);
            return false;
        }
    }
    
    public TeacherModel? GetTeacherById(int id, int? termId = null)
    {
        try
        {
            var connection = _db.GetConnection();
            string sql = @$" SELECT t.id, t.avatar, t.fullname, t.birthday, t.gender, t.address, t.phone, t.email, t.status,
                                   COALESCE(dd.department_id, 0) AS department_id, 
                                   COALESCE(d.name, 'Chưa có bộ môn') AS department_name,
                                   COALESCE(ac.class_id, 0) AS class_id,
                                   COALESCE(c.name, 'Chưa có lớp') AS class_name
                            FROM teachers t
                            LEFT JOIN department_details dd ON dd.teacher_id = t.id
                            LEFT JOIN departments d ON d.id = dd.department_id
                            LEFT JOIN assign_class_teachers act ON act.teacher_id = t.id
                            LEFT JOIN assign_classes ac ON ac.id = act.assign_class_id
                            LEFT JOIN classes c ON c.id = ac.class_id
                            LEFT JOIN terms ter ON ter.id = ac.term_id AND ter.id = @termId
                            WHERE t.id = @id
                            LIMIT 1";
            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@termId", termId);

            var adapter = new MySqlDataAdapter(command);
            var dt = new DataTable();
            adapter.Fill(dt);

            DataRow data = dt.Rows[0];
            return new TeacherModel
            {
                Id = Convert.ToInt32(data["id"]),
                Avatar = data["avatar"].ToString()!,
                Name = data["fullname"].ToString()!,
                Birthday = Convert.ToDateTime(data["birthday"]).ToString("dd/MM/yyyy"),
                Gender = data["gender"].ToString()!,
                Address = data["address"].ToString()!,
                Phone = data["phone"].ToString()!,
                Email = data["email"].ToString()!,
                Status = Convert.ToInt32(data["status"]),
                DepartmentId = Convert.ToInt32(data["department_id"]),
                DepartmentName = data["department_name"].ToString()!,
                ClassId = Convert.ToInt32(data["class_id"]),
                ClassName = data["class_name"].ToString()!,               
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể tìm giáo viên: " + e);
            return null;
        }
    }
    
    


}

