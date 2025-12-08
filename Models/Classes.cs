namespace Models;

public class Classes
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Area { get; set; } = "";
    public string Room { get; set; } = "";
    public int Status { get; set; } = 1;
    public int Assign_class_Id { get; set; }

    public Classes(int ma, int Assign_class_Id, string ten, string phong)
    {
        Id = ma;
        this.Assign_class_Id = Assign_class_Id;
        Name = ten;
        Room = phong;
    }
}