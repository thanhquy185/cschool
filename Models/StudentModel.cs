public class Student
{
    public string Fullname { get; set; }
    public int Age { get; set; }

    public Student(string Fullname, int Age)
    {
        this.Fullname = Fullname;
        this.Age = Age;
    }
}
