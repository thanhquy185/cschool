using Avalonia.Media.Imaging;
using Avalonia.Platform;

public class FunctionModel
{
    // Properties
    // - Cột trong bảng dữ liệu
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsTeacherFunction { get; set; }
    public string Actions { get; set; }

    // Constructors
    public FunctionModel() { }
    public FunctionModel(int Id, string Name, bool IsTeacherFunction, string Actions)
    {
        this.Id = Id;
        this.Name = Name;
        this.IsTeacherFunction = IsTeacherFunction;
        this.Actions = Actions;
    }
}
