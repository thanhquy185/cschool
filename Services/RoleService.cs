using System.Data;

namespace Services;

public class RoleService
{
    private readonly DBService _db;

    public RoleService(DBService db)
    {
        _db = db;
    }

    public List<RoleModel> GetRoles()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM roles");
        var list = new List<RoleModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new RoleModel(
                (int)row["id"],
                row["name"].ToString()!,
                row["status"].ToString()!
            ));
        }

        return list;
    }

    public List<RoleModel> GetRolesActive()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM roles WHERE status = 'Hoạt động'");
        var list = new List<RoleModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new RoleModel(
                (int)row["id"],
                row["name"].ToString()!,
                row["status"].ToString()!
            ));
        }

        return list;
    }

    public RoleModel? GetRoleById(int id)
    {
        var dt = _db.ExecuteQuery($"SELECT * FROM roles WHERE id = {id} LIMIT 1");
        if (dt.Rows.Count > 0)
        {
            var row = dt.Rows[0];
            return new RoleModel(
                (int)row["id"],
                row["name"].ToString()!,
                row["status"].ToString()!
            );
        }
        return null;
    }

    public int GetIdLastRole()
    {
        var dt = _db.ExecuteQuery("SELECT id FROM roles ORDER BY id DESC LIMIT 1");
        if (dt.Rows.Count > 0)
            return System.Convert.ToInt32(dt.Rows[0]["id"]);
        return 0;
    }

    public int CreateRole(RoleModel role)
    {
        string sql = $"INSERT INTO roles (name, status) VALUES ('{role.Name}', '{role.Status}')";
        return _db.ExecuteNonQuery(sql);
    }

    public int UpdateRole(RoleModel role)
    {
        string sql = $"UPDATE roles SET name = '{role.Name}' WHERE id = {role.Id}";
        return _db.ExecuteNonQuery(sql);
    }

    public int LockRole(RoleModel role)
    {
        string sql = $"UPDATE roles SET " +
                     $"status = '{role.Status}' " +
                     $"WHERE id = {role.Id}";

        return _db.ExecuteNonQuery(sql);
    }
}
