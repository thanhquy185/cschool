namespace cschool.Models;

public class Teachers
{
    public int Id { get; set; }
    public string Birthday { get; set; } = "";
    public string Avatar { get; set; } = "";
    public string Gender { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string Department { get; set; } = ""; // Thêm dòng này
    public int Status { get; set; } = 1;
  

    public Teachers(int id, string name, string gender, string birtday, string email, string phone, string address, string department)
    {
        Id = id;
        Name = name;
        Birthday = birtday;
        Gender = gender;
        Email = email;
        Phone = phone;
        Address = address;
        Department = department;
    }
    public Teachers(int id, string name, string department)
    {
        Id = id;
        Name = name;
        Department = department;
    }
}