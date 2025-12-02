using System;
using System.Collections.ObjectModel;
using System.Data;
using cschool.Models;
using MySql.Data.MySqlClient;

namespace cschool.Services;

public class TermService
{
    private readonly DBService _db;

    public TermService(DBService dbService)
    {
        _db = dbService;
    }

    public ObservableCollection<TermModel> GetCurrentTerm()
    {
        try
        {
            var terms = new ObservableCollection<TermModel>();
            string query = @"SELECT id, name, start_date, end_date, status 
                            FROM terms
                            WHERE status = 1";

            var results = _db.ExecuteQuery(query);

            foreach (DataRow data in results.Rows)
            {
                var term = new TermModel
                {
                    Id = Convert.ToInt32(data["id"]),
                    Name = data["name"].ToString()!,
                    StartDate = Convert.ToDateTime(data["start_date"]).ToString("dd/MM/yyyy")!,
                    EndDate = Convert.ToDateTime(data["end_date"]).ToString("dd/MM/yyyy")!,
                    Status = Convert.ToInt32(data["status"])
                };
                terms.Add(term);
            }

            return terms;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi lấy danh sách học kỳ: {ex.Message}");
            return new ObservableCollection<TermModel>();
        }
    }

    // Lấy học kỳ gần nhất (nếu không có học kỳ hiện tại)
    public TermModel? GetLatestTerm()
    {
        try
        {
            string query = @"SELECT id, name, start_date, end_date, status 
                            FROM terms
                            WHERE status = 1
                            ORDER BY start_date DESC
                            LIMIT 1";

            var results = _db.ExecuteQuery(query);

            if (results.Rows.Count > 0)
            {
                DataRow data = results.Rows[0];
                return new TermModel
                {
                    Id = Convert.ToInt32(data["id"]),
                    Name = data["name"].ToString()!,
                    StartDate = Convert.ToDateTime(data["start_date"]).ToString("dd/MM/yyyy"),
                    EndDate = Convert.ToDateTime(data["end_date"]).ToString("dd/MM/yyyy"),
                    Status = Convert.ToInt32(data["status"])
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi lấy học kỳ gần nhất: {ex.Message}");
            return null;
        }
    }
    public TermModel? GetTermByDate(DateTime date)
    {
        try
        {
            string query = @"SELECT id, name, start_date, end_date, status 
                            FROM terms
                            WHERE status = 1
                            AND @date BETWEEN start_date AND end_date
                            LIMIT 1";

            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@date", date.ToString("dd/MM/yyyy"));

            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                return new TermModel
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    StartDate = reader.GetDateTime("start_date").ToString("dd/MM/yyyy"),
                    EndDate = reader.GetDateTime("end_date").ToString("dd/MM/yyyy"),
                    Status = reader.GetInt32("status")
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi lấy học kỳ theo ngày: {ex.Message}");
            return null;
        }
    }
}
