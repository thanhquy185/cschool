using System.Data;

namespace Services;

public class UserService
{
    private readonly DBService _db;

    public UserService(DBService db)
    {
        _db = db;
    }

    public bool UserIsExistsByUsername(string username)
    {
        var dt = _db.ExecuteQuery($"SELECT * FROM users WHERE username = '{username}'");
        return dt.Rows.Count > 0;

    }

    public int GetIdLastUser()
    {
        // Console.WriteLine(123);
        var dt = _db.ExecuteQuery("SELECT id FROM users ORDER BY id DESC LIMIT 1");
        if (dt.Rows.Count > 0)
            return System.Convert.ToInt32(dt.Rows[0]["id"]);
        return 0;
    }

    public List<UserModel> GetUsers()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM users");
        var list = new List<UserModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new UserModel(
                (int)row["id"],
                row["avatar"].ToString()!,
                row["username"].ToString()!,
                row["password"].ToString()!,
                (int)row["role_id"],
                row["fullname"].ToString()!,
                row["phone"].ToString()!,
                row["email"].ToString()!,
                row["address"].ToString()!,
                row["status"].ToString()!
            ));
        }

        return list;
    }

    public int CreateUser(UserModel user)
    {
        try 
        {
            string sql = $"INSERT INTO cschool.users (avatar, username, password, role_id, fullname, phone, email, address, status) " +
                     $"VALUES ('{user.Avatar}', '{user.Username}', '{user.Password}', {user.RoleId}, " +
                     $"'{user.Fullname}', '{user.Phone}', '{user.Email}', '{user.Address}', '{user.Status}')";
            return _db.ExecuteNonQuery(sql);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in CreateUser: " + ex.Message);
            return -1;
        }
    }

    public int UpdateUser(UserModel user)
    {
        string sql = $"UPDATE users SET " +
                     $"avatar = '{user.Avatar}', " +
                     $"role_id = {user.RoleId}, " +
                     $"fullname = '{user.Fullname}', " +
                     $"phone = '{user.Phone}', " +
                     $"email = '{user.Email}', " +
                     $"address = '{user.Address}' " +
                     $"WHERE id = {user.Id}";

        return _db.ExecuteNonQuery(sql);
    }

    public int LockUser(UserModel user)
    {
        string sql = $"UPDATE users SET " +
                     $"status = '{user.Status}' " +
                     $"WHERE id = {user.Id}";

        return _db.ExecuteNonQuery(sql);
    }
}
