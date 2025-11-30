using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using cschool.Models;
using System.Threading.Tasks;
using cschool.Utils;


namespace cschool.Services;
public class StatisticalService
{
    private readonly DBService _dbService;

    public StatisticalService(DBService dbService)
    {
        _dbService = dbService;
    }

    public int  GetCountStudents()
        {
            try
            {
                string sql = "SELECT COUNT(*) as total FROM students WHERE status = 1 ";
                var connection = _dbService.GetConnection();
                var cmd = new MySqlCommand(sql,connection);
                int total = Convert.ToInt32(cmd.ExecuteScalar());
                return total;
            }catch(Exception e)
            {
                Console.WriteLine("Không thể đếm số lượng học sinh" + e);
                return 0;
            }
        }
      public int  GetCountTeacher()
        {
            try
            {
                string sql = "SELECT COUNT(*) as total FROM teachers WHERE status = 1 ";
                var connection = _dbService.GetConnection();
                var cmd = new MySqlCommand(sql,connection);
                int total = Convert.ToInt32(cmd.ExecuteScalar());
                return total;
            }catch(Exception e)
            {
                Console.WriteLine("Không thể đếm số lượng giáo viên" + e);
                return 0;
            }
        }

        public int  GetCountClass()
        {
            try
            {
                string sql = "SELECT COUNT(*) as total FROM classes WHERE status = 1 ";
                var connection = _dbService.GetConnection();
                var cmd = new MySqlCommand(sql,connection);
                int total = Convert.ToInt32(cmd.ExecuteScalar());
                return total;
            }catch(Exception e)
            {
                Console.WriteLine("Không thể đếm số lượng lớp học" + e);
                return 0;
            }
        }
         public int  GetCountSubject()
        {
            try
            {
                string sql = "SELECT COUNT(*) as total FROM subjects WHERE status = 1 ";
                var connection = _dbService.GetConnection();
                var cmd = new MySqlCommand(sql,connection);
                int total = Convert.ToInt32(cmd.ExecuteScalar());
                return total;
            }catch(Exception e)
            {
                Console.WriteLine("Không thể đếm số lượng lớp học" + e);
                return 0;
            }
        }

    public List<Term> GetTerms()
    {
        try
        {
            List<Term> ds = new List<Term>();
            string sql = "SELECT * FROM terms WHERE status = 1 ";
            var dt = _dbService.ExecuteQuery(sql);
            foreach(DataRow data in dt.Rows)
            {
                ds.Add(new Term
                {
                    Id = (int)data["id"],
                    Name = data["name"].ToString()!,
                    Year = (int)data["year"],
                    Start = Convert.ToDateTime(data["start_date"]),
                    End = Convert.ToDateTime(data["end_date"]),
                });
            }
            return ds;
        }catch(Exception e)
        {
            Console.WriteLine("Không thể lấy các kì học"+ e);
            return new List<Term>();
        }
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
            
        }

    }
    public List<Statistical> SearchStatistics(int id)
    {
        try
        {
            List<Statistical> statistics = new List<Statistical>();

            string query = @"SELECT t.assign_class_id, t.student_id,t.gpa,t.conduct_level,t.academic,ac.term_id,
                            c.name AS className, s.fullname AS studentName
                            FROM term_gpa t
                            JOIN students s ON t.student_id = s.id
                            JOIN assign_classes ac ON t.assign_class_id = ac.id
                            JOIN classes c ON ac.class_id = c.id 
                            WHERE ac.term_id = "+ id;
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
                stat.Term_id = (int)row["term_id"];

                statistics.Add(stat);
            }
            return statistics;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching statistics: " + ex.Message);
    
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
          
        }

    }
    

}
