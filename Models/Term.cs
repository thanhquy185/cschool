using System;

namespace cschool.Models;

public class Term
{
    public int Id{get; set;}
    public string Name{get; set;} = "";
    public int Year{get; set;}
    public DateTime Start{get; set;}
    public DateTime End{get; set;}
    public string DisplayText => $"{Name} - Năm {Year} ({Start:dd/MM/yyyy} - {End:dd/MM/yyyy})";
    public string DisplayTextTermYear => $"{Name} - Năm {Year}";
}