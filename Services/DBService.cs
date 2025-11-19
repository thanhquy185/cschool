using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace cschool.Services
{
    public class DBService
    {
        private readonly string _connectionString;

        public DBService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Thực thi SELECT
        public DataTable ExecuteQuery(string query)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            using var adapter = new MySqlDataAdapter(command);

            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        // Thực thi INSERT / UPDATE / DELETE
        public int ExecuteNonQuery(string query)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            return command.ExecuteNonQuery();
        }

        // ---- Thêm ExecuteScalar (trả object) ----
        public object? ExecuteScalar(string sql)
        {
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            return cmd.ExecuteScalar();
        }

        // ---- Hoặc Generic tiện dụng (trả T) ----
        public T ExecuteScalar<T>(string sql)
        {
            using var conn = new MySqlConnection(_connectionString);
            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            var result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                return default!;
            return (T)Convert.ChangeType(result, typeof(T));
        }
    }
}
