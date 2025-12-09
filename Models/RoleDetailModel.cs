using Avalonia.Media.Imaging;
using Avalonia.Platform;

public class RoleDetailModel
{
    // Properties
    // - Cột trong bảng dữ liệu
    public int RoleId { get; set; }
    public int FunctionId { get; set; }
    public string Action { get; set; }

    // Constructors
    public RoleDetailModel() { }
    public RoleDetailModel(int RoleId, int FunctionId, string Action)
    {
        this.RoleId = RoleId;
        this.FunctionId = FunctionId;
        this.Action = Action;
    }
}
