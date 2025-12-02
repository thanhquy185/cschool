using System;

namespace cschool.Models;
public class TermModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
    public int Status { get; set; } = 1;

    // public TermModel(int id, string name, DateTime startDate, DateTime endDate, int status)
    // {
    //     Id = id;
    //     Name = name;
    //     StartDate = startDate;
    //     EndDate = endDate;
    //     Status = status;
    // }
}