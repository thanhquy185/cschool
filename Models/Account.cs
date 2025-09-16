using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

public class Account
{
    // Properties
    public int Id { get; set; }
    public string Avatar { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public string Fullname { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Status { get; set; }

    // Constructors
    public Account(int Id, string Avatar, string Username, string Password,
        int RoleId, string Fullname, string Phone, string Email,
        string Address, string Status)
    {
        this.Id = Id;
        this.Avatar = Avatar;
        this.Username = Username;
        this.Password = Password;
        this.RoleId = RoleId;
        this.Fullname = Fullname;
        this.Phone = Phone;
        this.Email = Email;
        this.Address = Address;
        this.Status = Status;
    }

    // Methods
    public Bitmap AvatarImage
    {
        get
        {
            var uri = new Uri($"avares://cschool/Assets/Images/Others/no-image.png");
            if (!string.IsNullOrEmpty(Avatar))
            {
                uri = new Uri($"avares://cschool/Assets/Images/Accounts/{Avatar}");
            }
            return new Bitmap(AssetLoader.Open(uri));
        }
    }
}
