using System;

public class StudentModel
{
    public int Id { get; set; }
    public string Avatar { get; set; }
    public string Fullname { get; set; }
    public DateTime BirthDay { get; set; }
    public string Gender { get; set; }
    public string Ethnicity { get; set; }
    public string Religion { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string LearnYear { get; set; }
    public string LearnStatus { get; set; }
    public sbyte Status { get; set; }

    public StudentModel() {}

    public StudentModel(int Id, string Fullname, string Avatar, DateTime BirthDay,
        string Gender, string Ethnicity, string Religion, string Phone, string Email,
        string Address, string LearnYear, string LearnStatus, sbyte Status)
    {
        this.Id = Id;
        this.Fullname = Fullname;
        this.Avatar = Avatar;
        this.BirthDay = BirthDay;
        this.Gender = Gender;
        this.Ethnicity = Ethnicity;
        this.Religion = Religion;
        this.Phone = Phone;
        this.Email = Email;
        this.Address = Address;
        this.LearnYear = LearnYear;
        this.LearnStatus = LearnStatus;
        this.Status = Status;
    }
}
