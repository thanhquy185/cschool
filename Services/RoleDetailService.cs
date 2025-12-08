using System.Data;

namespace Services;

public class RoleDetailService
{
    private readonly DBService _db;

    public RoleDetailService(DBService db)
    {
        _db = db;
    }

    public List<RoleDetailModel> GetRoleDetailsByRoleId(int roleId)
    {
        var dt = _db.ExecuteQuery($"SELECT * FROM role_details WHERE role_id = {roleId}");
        var list = new List<RoleDetailModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new RoleDetailModel(
                (int)row["role_id"],
                (int)row["function_id"],
                row["action"].ToString()!
            ));
        }

        return list;
    }

    public int AssignFunctionToRole(RoleDetailModel rd)
    {
        string sql = $"INSERT INTO role_details (role_id, function_id, action) " +
                     $"VALUES ({rd.RoleId}, {rd.FunctionId}, '{rd.Action}')";
        return _db.ExecuteNonQuery(sql);
    }

    public int UpdateRoleFunction(RoleDetailModel rd)
    {
        string sql = $"UPDATE role_details SET action = '{rd.Action}' " +
                     $"WHERE role_id = {rd.RoleId} AND function_id = {rd.FunctionId}";
        return _db.ExecuteNonQuery(sql);
    }

    public int RemoveFunctionFromRole(int roleId, int functionId)
    {
        string sql = $"DELETE FROM role_details WHERE role_id = {roleId} AND function_id = {functionId}";
        return _db.ExecuteNonQuery(sql);
    }

    // Kiểm tra quyền
    public bool HasPermission(int roleId, int functionId, string action)
    {
        string sql =
            "SELECT rd.* " +
            "FROM role_details rd " +
            $"WHERE rd.role_id = {roleId} AND rd.function_id = {functionId} AND rd.action = '{action}'";
        var dt = _db.ExecuteQuery(sql);
        return dt.Rows.Count > 0;
    }
}
