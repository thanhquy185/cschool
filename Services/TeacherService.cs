using System;
using System.Collections.Generic;
using System.Data;
using cschool.Models;
using MySql.Data.MySqlClient;

namespace cschool.Services;

public class TeacherService
{
    // Phương thức và thuộc tính liên quan đến quản lý giáo viên sẽ được định nghĩa ở đây.

    private readonly DBService _dbService;
    public TeacherService(DBService dbService)
    {
        _dbService = dbService;
    }

    public List<Teachers> GetTeachers()
    {
        try
        {
            List<Teachers> ds = new List<Teachers>();
            string sql = @" select t.id , t.fullname,t.birthday,t.gender,t.address,t.phone,t.email, d.name as department_name
                            FROM teachers t
                            JOIN department_details dd ON dd.teacher_id = t.id
                            JOIN departments d ON d.id = dd.department_id ";
            var result = _dbService.ExecuteQuery(sql);
            foreach (DataRow data in result.Rows)
            {
                ds.Add(new Teachers(
                    (int)data["id"],
                    data["fullname"].ToString()!,
                    data["birthday"].ToString()!,
                    data["gender"].ToString()!,
                    data["address"].ToString()!,
                    data["phone"].ToString()!,
                    data["email"].ToString()!,
                    data["department_name"].ToString()!
                ));

            }
            return ds;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTeachers: {ex.Message}");
            return new List<Teachers>();
        }

    }

    public bool AddTeacher(Teachers t)
    {
        try
        {
            var connection = _dbService.GetConnection();
            string sql = @"INSERT INTO teachers (fullname, birthday, gender, address, phone, email,status) 
                       VALUES (@fullname, @birthday, @gender, @address, @phone, @email,@status);
                       ";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@fullname", t.Name);
            command.Parameters.AddWithValue("@birthday", t.Birthday);
            command.Parameters.AddWithValue("@gender", t.Gender);
            command.Parameters.AddWithValue("@address", t.Address);
            command.Parameters.AddWithValue("@phone", t.Phone);
            command.Parameters.AddWithValue("@email", t.Email);
            command.Parameters.AddWithValue("@status", t.Status);

            return command.ExecuteNonQuery() > 0;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"lỗi không thể thêm giáo viên: {ex.Message}");
            return false;
        }
    }


    public bool DeleteTeacher(int id)
    {
        try
        {
            var connection = _dbService.GetConnection();
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

    public bool UpdateTeacher(Teachers t)
    {
         try
        {
            var connection = _dbService.GetConnection();
            string sql = @"UPDATE teachers SET fullname = @fullname, birthday = @birthday, gender = @gender, address = @address,
            phone = @phone, email = @email where id = @id";

            var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@fullname", t.Name);
            command.Parameters.AddWithValue("@birthday",t.Birthday);
            command.Parameters.AddWithValue("@gender", t.Gender);
            command.Parameters.AddWithValue("@address", t.Address);
            command.Parameters.AddWithValue("@phone", t.Phone);
            command.Parameters.AddWithValue("@email", t.Email);
            command.Parameters.AddWithValue("@id", t.Id);
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Lỗi không thể xóa giáo viên: " + e);
            return false;
        }
    }
    
    


}

