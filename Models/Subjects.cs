using System;

namespace cschool.Models;

public class Subjects
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name_Subject { get; set; } = "";
    public int Credits { get; set; }
    public string Department { get; set; } = "";
    public Subjects(int ma, string ten)
    {
        Id = ma;
        Name_Subject = ten;
    }
}