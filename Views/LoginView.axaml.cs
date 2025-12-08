using System;
using Avalonia;
using Avalonia.Controls;

namespace Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        Console.WriteLine("LoginView đang khởi tạo...");
    }
}