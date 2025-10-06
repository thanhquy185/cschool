using System;
using System.Collections.Generic;
using System.Data;

namespace cschool.Services;

public class StudentService
{
    private readonly DBService _db;

    public StudentService(DBService db)
    {
        _db = db;
    }

    // public bool UserIsExistsByUsername(string username)
    // {
    //     var dt = _db.ExecuteQuery($"SELECT * FROM cschool.users WHERE username = '{username}'");
    //     return dt.Rows.Count > 0;

    // }

    // public int GetIdLastUser()
    // {
    //     // Console.WriteLine(123);
    //     var dt = _db.ExecuteQuery("SELECT id FROM cschool.users ORDER BY id DESC LIMIT 1");
    //     if (dt.Rows.Count > 0)
    //         return System.Convert.ToInt32(dt.Rows[0]["id"]);
    //     return 0;
    // }

    public List<StudentModel> GetStudents()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM cschool.students");
        var list = new List<StudentModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new StudentModel(
                (int)row["id"],
                row["fullname"].ToString()!,
                row["avatar"].ToString()!,
                row["birthday"] == DBNull.Value 
                    ? DateTime.MinValue 
                    : Convert.ToDateTime(row["birthday"]),
                row["gender"].ToString()!,
                row["ethnicity"].ToString()!,
                row["religion"].ToString()!,
                row["phone"].ToString()!,
                row["email"].ToString()!,
                row["address"].ToString()!,
                row["learn_year"].ToString()!,
                row["learn_status"].ToString()!,
                (sbyte)row["status"]
            ));
        }

        return list;
    }

    // public int CreateUser(UserModel user)
    // {
    //     string sql = $"INSERT INTO cschool.users (avatar, username, password, role_id, fullname, phone, email, address, status) " +
    //                  $"VALUES ('{user.Avatar}', '{user.Username}', '{user.Password}', {user.RoleId}, " +
    //                  $"'{user.Fullname}', '{user.Phone}', '{user.Email}', '{user.Address}', '{user.Status}')";
    //     return _db.ExecuteNonQuery(sql);
    // }

    // public int UpdateUser(UserModel user)
    // {
    //     string sql = $"UPDATE cschool.users SET " +
    //                  $"avatar = '{user.Avatar}', " +
    //                  $"role_id = {user.RoleId}, " +
    //                  $"fullname = '{user.Fullname}', " +
    //                  $"phone = '{user.Phone}', " +
    //                  $"email = '{user.Email}', " +
    //                  $"address = '{user.Address}' " +
    //                  $"WHERE id = {user.Id}";

    //     return _db.ExecuteNonQuery(sql);
    // }

    // public int LockUser(UserModel user)
    // {
    //     string sql = $"UPDATE cschool.users SET " +
    //                  $"status = '{user.Status}' " +
    //                  $"WHERE id = {user.Id}";

    //     return _db.ExecuteNonQuery(sql);
    // }
}
