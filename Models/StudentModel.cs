public class Student
{
    public int Id{ get; set; }
    public string Fullname { get; set; }
    public int Age { get; set; }

    public Student(string Fullname, int Age)
    {
        this.Fullname = Fullname;
        this.Age = Age;
    }
    public Student(int id,string Fullname)
    {
        this.Id = id;
        this.Fullname = Fullname;
        
    }
}
