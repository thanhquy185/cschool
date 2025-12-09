using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services;
using System;
using Utils;

namespace ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _username;
    [ObservableProperty]
    private string _password;
    [ObservableProperty]
    private string _errorUsername;
    [ObservableProperty]
    private string _errorPassword;

    public Action<UserModel> OnLoginSuccess; // Trả về thông tin người dùng

    [RelayCommand]
    public async void Login()
    {
        this.ErrorUsername = "";
        this.ErrorPassword = "";
        if (Rules.ruleRequiredForTextBox(Username ?? ""))
        {
            this.ErrorUsername = "Tên tài khoản được để trống !";
        }
        if (Rules.ruleRequiredForTextBox(Password ?? ""))
        {
            this.ErrorPassword = "Mật khẩu được để trống !";
        }
        if (ErrorUsername == "Tên tài khoản được để trống !" || ErrorPassword == "Mật khẩu được để trống !") return;

        var user = AppService.UserService.Login(Username, Password);
        if (user != null && user.Status == "Hoạt động")
        {
            this.Username = "";
            this.Password = "";
            await MessageBoxUtil.ShowSuccess("Đăng nhập thành công!", owner: null);
            OnLoginSuccess?.Invoke(user);
        }
        else if (user != null && user.Status == "Tạm dừng")
        {
            await MessageBoxUtil.ShowError("Tài khoản đã bị khoá!", owner: null);
            return;
        }
        else
        {
            await MessageBoxUtil.ShowError("Tên đăng nhập hoặc mật khẩu không đúng!", owner: null);
            return;
        }
    }
}
