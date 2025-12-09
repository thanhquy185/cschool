namespace Models
{
    public class TermClassModel
    {
        public int Id { get; set; }               
        public string TermName { get; set; } = "";   
        public string Year { get; set; } = "";   
        public string StartDate { get; set; } = ""; 
        public string EndDate { get; set; } = "";  
        public short Status { get; set; } = 1;      
        public string DisplayName => $"{Year} - {TermName}";
    }
}
