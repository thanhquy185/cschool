using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services;
using System;

namespace ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _errorMessage;

    public Action<UserModel> OnLoginSuccess; // Trả về thông tin người dùng

    [RelayCommand]
    public void Login()
    {
        var user = AppService.UserService.Login(Username, Password);
        if (user != null)
        {
            OnLoginSuccess?.Invoke(user); // thông báo MainWindowViewModel kèm thông tin user
        }
        else
        {
            ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
        }
    }
}
