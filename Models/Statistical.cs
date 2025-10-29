
namespace cschool.Models;

public class Statistical
{
    public int Assign_class_id { get; set; }
    public string Class_name { get; set; } = "";
    public int Student_id { get; set; }
    public string StudentName { get; set; } = "";
    public float Gpa { get; set; }
    public string ConductLevel { get; set; } = "";
    public string Academic{ get; set; } = "";
    public Statistical(int assign_class_id, string class_name, int student_id, string studentName, float gpa, string conductLevel, string academic)
    {
        this.Assign_class_id = assign_class_id;
        this.Class_name = class_name;
        this.Student_id = student_id;
        this.StudentName = studentName;
        Gpa = gpa;
        this.ConductLevel = conductLevel;
        Academic = academic;
    }
    public Statistical(int ma,string studentName, float gpa, string conductLevel)
    {
        this.Student_id = ma;
        this.StudentName = studentName;
        Gpa = gpa;
        this.ConductLevel = conductLevel;
    }
    
   public Statistical(int maHs, float gpa, string conductLevel, string academic)
    {
        Student_id = maHs;
        Gpa = gpa;
        ConductLevel = conductLevel;
        Academic = academic;
    }
}