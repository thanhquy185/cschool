using System;
using System.Collections.Generic;
using System.Data;
using cschool.Models;
using MySql.Data.MySqlClient;

namespace cschool.Services;

public class DepartmentService
{
    private readonly DBService _db;

    public DepartmentService(DBService dbService)
    {
        _db = dbService;
    }

    public List<DepartmentModel> GetDepartments()
    {
        try
        {
            var departments = new List<DepartmentModel>();
            string query = @"SELECT id, subject_id, name, description, status 
                            FROM departments
                            WHERE status = 1";

            var results = _db.ExecuteQuery(query);

            foreach (DataRow data in results.Rows)
            {
                var dept = new DepartmentModel(
                    Convert.ToInt32(data["id"]),
                    Convert.ToInt32(data["subject_id"]),
                    data["name"].ToString()!,
                    data["description"].ToString()!,
                    Convert.ToInt32(data["status"])
                );
                departments.Add(dept);
            }

            return departments;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi lấy danh sách bộ môn: {ex.Message}");
            return new List<DepartmentModel>();
        }
    }
    
}