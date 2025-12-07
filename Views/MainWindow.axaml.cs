using System;
using Avalonia;
using Avalonia.Controls;

namespace Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Console.WriteLine("MainWindow đang khởi tạo...");
    
    #if DEBUG
    this.AttachDevTools();
    #endif
    }
}