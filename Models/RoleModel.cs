using Avalonia.Media.Imaging;
using Avalonia.Platform;

public class RoleModel
{
    // Properties
    // - Cột trong bảng dữ liệu
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }

    // Constructors
    public RoleModel() { }
    public RoleModel(int Id, string Name, string Status)
    {
        this.Id = Id;
        this.Name = Name;
        this.Status = Status;
    }
}
