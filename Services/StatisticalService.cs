using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using cschool.Models;


namespace cschool.Services;
public class StatisticalService
{
    private readonly DBService _dbService;

    public StatisticalService(DBService dbService)
    {
        _dbService = dbService;
    }

    // Example method to get statistics
    public List<Statistical> GetStatistics()
    {
        try
        {
            List<Statistical> statistics = new List<Statistical>();

            string query = @"SELECT t.assign_class_id, t.student_id,t.gpa,t.conduct_level,t.academic,
                            c.name AS className, s.fullname AS studentName
                            FROM term_gpa t
                            JOIN students s ON t.student_id = s.id
                            JOIN assign_classes ac ON t.assign_class_id = ac.id
                            JOIN classes c ON ac.class_id = c.id;";
            var dt = _dbService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                Statistical stat = new Statistical(
                    Convert.ToInt32(row["assign_class_id"]),
                    row["className"].ToString() ?? "",
                    Convert.ToInt32(row["student_id"]),
                    row["studentName"].ToString() ?? "",
                    Convert.ToSingle(row["gpa"]),
                    row["conduct_level"].ToString() ?? "",
                    row["academic"].ToString() ?? ""
                );

                statistics.Add(stat);
            }
            return statistics;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching statistics: " + ex.Message);
            return new List<Statistical>();
        }

    }

    public List<Statistical> DetailStatistic(string rank)
    {
        try
        {
            List<Statistical> statistics = new List<Statistical>();

            string query = @"SELECT t.assign_class_id, t.student_id,t.gpa,t.conduct_level,
                            c.name AS className, s.fullname AS studentName
                            FROM term_gpa t
                            JOIN students s ON t.student_id = s.id
                            JOIN assign_classes ac ON t.assign_class_id = ac.id
                            JOIN classes c ON ac.class_id = c.id 
                            WHERE t.academic = '" + rank + "';";
            var dt = _dbService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                Statistical stat = new Statistical(
                    Convert.ToInt32(row["student_id"]),
                    row["studentName"].ToString() ?? "",
                    Convert.ToSingle(row["gpa"]),
                    row["conduct_level"].ToString() ?? ""
                );

                statistics.Add(stat);
            }
            return statistics;

        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching detailed statistics: " + ex.Message);
            return new List<Statistical>();
        }
    }
    
    public List<Statistical> SearchStatistic(string semester,string year)
    {
        try
        {
            List<Statistical> statistics = new List<Statistical>();
            
            string query = @"SELECT t.assign_class_id, t.student_id,t.gpa,t.conduct_level,t.academic,
                            c.name AS className, s.fullname AS studentName
                            FROM term_gpa t
                            JOIN students s ON t.student_id = s.id
                            JOIN assign_classes ac ON t.assign_class_id = ac.id
                            JOIN classes c ON ac.class_id = c.id
                            JOIN terms ON ac.term_id = terms.id 
                            WHERE terms.name = @name AND terms.year = @year ";
            var connection = _dbService.GetConnection();
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@name", semester);
            command.Parameters.AddWithValue("@year", year);
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                statistics.Add(new Statistical(
                    (int)reader["assign_class_id"],
                    reader["className"].ToString()!,
                    (int)reader["student_id"],
                    reader["studentName"].ToString()!,
                    (float)reader["gpa"],
                    reader["conduct_level"].ToString()!,
                    reader["academic"].ToString()!

                ));
            }
            return statistics;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching statistics: " + ex.Message);
            return new List<Statistical>();
        }

    }
    

}
