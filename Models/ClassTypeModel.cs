namespace Models;

public class ClassTypeModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ClassTypeModel() { }
    public ClassTypeModel(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
