namespace Models;

public class DepartmentModel
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Status { get; set; } = 1;

    public DepartmentModel(int id, int subjectId,  string name, string description,  int status)
    {
        Id = id;
        SubjectId = subjectId;
        Name = name;
        Description = description;
        Status = status;
    }
}