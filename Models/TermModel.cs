namespace Models;

public class TermModel
{
    public int Id{get; set;}
    public string Name{get; set;} = "";
    public int Year{get; set;}
    public string LearnYear {get; set;} = "";
    public DateTime Start{get; set;}
    public DateTime End{get; set;}
    public int Status{get; set;}
    public string DisplayText => $"{Name} Năm {LearnYear} ({Start:dd/MM/yyyy} - {End:dd/MM/yyyy})";
    public string DisplayTextTermYear => $"{Name} Năm {LearnYear}";
}