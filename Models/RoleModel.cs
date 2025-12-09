using Avalonia.Media.Imaging;
using Avalonia.Platform;

public class RoleModel
{
    // Properties
    // - Cột trong bảng dữ liệu
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public List<RoleDetailModel> RoleDetails { get; set; }

    // Constructors
    public RoleModel() { }
    public RoleModel(int Id, string Name, string Status)
    {
        this.Id = Id;
        this.Name = Name;
        this.Status = Status;
    }
    public RoleModel(int Id, string Name, string Status, List<RoleDetailModel> RoleDetails)
    {
        this.Id = Id;
        this.Name = Name;
        this.Status = Status;
        this.RoleDetails = RoleDetails;
    }
}
