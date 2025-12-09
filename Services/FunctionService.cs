using System.Data;

namespace Services;

public class FunctionService
{
    private readonly DBService _db;

    public FunctionService(DBService db)
    {
        _db = db;
    }

    public List<FunctionModel> GetFunctions()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM functions");
        var list = new List<FunctionModel>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new FunctionModel(
                (int)row["id"],
                row["name"].ToString()!,
                (bool)row["is_teacher_function"]!,
                row["actions"].ToString()!
            ));
        }

        return list;
    }

    public FunctionModel? GetFunctionById(int id)
    {
        var dt = _db.ExecuteQuery($"SELECT * FROM functions WHERE id = {id} LIMIT 1");
        if (dt.Rows.Count > 0)
        {
            var row = dt.Rows[0];
            return new FunctionModel(
                (int)row["id"],
                row["name"].ToString()!,
                (bool)row["is_teacher_function"]!,
                row["actions"].ToString()!
            );
        }
        return null;
    }

    // public int CreateFunction(FunctionModel func)
    // {
    //     string sql = $"INSERT INTO functions (name, actions) VALUES ('{func.Name}', '{func.Actions}')";
    //     return _db.ExecuteNonQuery(sql);
    // }

    // public int UpdateFunction(FunctionModel func)
    // {
    //     string sql = $"UPDATE functions SET name = '{func.Name}', actions = '{func.Actions}' WHERE id = {func.Id}";
    //     return _db.ExecuteNonQuery(sql);
    // }

    // public int DeleteFunction(int id)
    // {
    //     string sql = $"DELETE FROM functions WHERE id = {id}";
    //     return _db.ExecuteNonQuery(sql);
    // }
}
